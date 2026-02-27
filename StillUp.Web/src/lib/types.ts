export interface MonitorEntry {
  id: number;
  name: string;
  url: string;
  date: string; // ISO string
  statusCode: string; // .NET HttpStatusCode enum name: "OK", "NotFound", etc.
}

export interface MonitorData {
  entries: Record<string, MonitorEntry[]>;
}

export type StatusCategory = "success" | "redirect" | "client-error" | "server-error" | "unknown";

export type ServiceStatus = "up" | "degraded" | "down" | "unknown";

export interface ServiceState {
  name: string;
  url: string;
  entries: MonitorEntry[]; // ordered newest-first
  uptimePercent: number;
  currentStatus: ServiceStatus;
  lastChecked: string | null;
}

export interface TimeRange {
  label: string;
  hours: number;
}

export type StatusFilter = "all" | "up" | "degraded" | "down";
