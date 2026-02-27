import { error } from "@sveltejs/kit";
import type { MonitorEntry } from "$lib/types";
import type { PageServerLoad } from "./$types";

export const load: PageServerLoad = async ({ fetch, url }) => {
  const hours = url.searchParams.get("hours") ?? "24";

  let monitorRes: Response;
  let namesRes: Response;
  try {
    [monitorRes, namesRes] = await Promise.all([
      fetch(`/api/monitors/recent/${hours}`),
      fetch("/api/monitors/names"),
    ]);
  } catch {
    error(503, "Could not reach the monitoring service");
  }

  if (!monitorRes.ok) {
    error(monitorRes.status, "Failed to load monitor data");
  }

  let monitorData: { entries: Record<string, MonitorEntry[]> };
  try {
    monitorData = (await monitorRes.json()) as { entries: Record<string, MonitorEntry[]> };
  } catch {
    error(502, "Unexpected response from monitor service");
  }

  let monitorNames: string[] = [];
  if (namesRes.ok) {
    const namesData = (await namesRes.json()) as { names?: string[] };
    monitorNames = namesData.names ?? Object.keys(monitorData.entries);
  } else {
    console.error(`Failed to load monitor names: ${namesRes.status}`);
    // Fall back to names derived from the historical data
    monitorNames = Object.keys(monitorData.entries);
  }

  return {
    initialData: monitorData,
    monitorNames,
    hours: parseInt(hours, 10),
  };
};
