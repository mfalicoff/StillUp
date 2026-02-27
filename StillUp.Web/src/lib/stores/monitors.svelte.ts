import type { MonitorEntry, ServiceState, StatusFilter, TimeRange } from "$lib/types";
import { calculateUptimePercent } from "$lib/utils/uptime";
import { deriveServiceStatus } from "$lib/utils/status";

export const TIME_RANGES: TimeRange[] = [
  { label: "1h", hours: 1 },
  { label: "6h", hours: 6 },
  { label: "24h", hours: 24 },
  { label: "7d", hours: 168 },
  { label: "30d", hours: 720 },
];

export const STATUS_FILTERS: StatusFilter[] = ["all", "up", "degraded", "down"];

function buildServiceState(name: string, entries: MonitorEntry[]): ServiceState {
  return {
    name,
    url: entries[0]?.url ?? "",
    entries,
    uptimePercent: calculateUptimePercent(entries),
    currentStatus: deriveServiceStatus(entries),
    lastChecked: entries[0]?.date ?? null,
  };
}

export const monitorsState = $state({
  services: {} as Record<string, ServiceState>,
  loading: true,
  error: null as string | null,
  sseConnected: false,
  filters: {
    search: "",
    timeRange: { label: "24h", hours: 24 } as TimeRange,
    status: "all" as StatusFilter,
  },
});

export function getFilteredServices(): ServiceState[] {
  return Object.values(monitorsState.services)
    .filter((s) => {
      const { search, status } = monitorsState.filters;
      if (search && !s.name.toLowerCase().includes(search.toLowerCase())) return false;
      if (status !== "all" && s.currentStatus !== status) return false;
      return true;
    })
    .sort((a, b) => a.name.localeCompare(b.name));
}

export function initServices(data: Record<string, MonitorEntry[]>): void {
  if (!data || typeof data !== "object") {
    console.error("[store] initServices called with invalid data", data);
    monitorsState.loading = false;
    return;
  }
  const services: Record<string, ServiceState> = {};
  for (const [name, entries] of Object.entries(data)) {
    services[name] = buildServiceState(name, entries);
  }
  monitorsState.services = services;
  monitorsState.loading = false;
  monitorsState.error = null;
}

export function upsertEntry(entry: MonitorEntry): void {
  const MAX_ENTRIES = 2000;
  const existing = monitorsState.services[entry.name];
  const entries = existing ? [entry, ...existing.entries].slice(0, MAX_ENTRIES) : [entry];
  monitorsState.services[entry.name] = buildServiceState(entry.name, entries);
}
