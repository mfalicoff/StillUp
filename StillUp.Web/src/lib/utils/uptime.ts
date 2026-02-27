import type { MonitorEntry, StatusCategory } from "$lib/types";
import { categorizeStatusCode } from "./status";

export interface Bucket {
  startTime: Date;
  endTime: Date;
  entries: MonitorEntry[];
  dominantCategory: StatusCategory;
}

const SEVERITY: Record<StatusCategory, number> = {
  "server-error": 4,
  "client-error": 3,
  redirect: 2,
  success: 1,
  unknown: 0,
};

/** Returns the highest-severity category found among the given entries. */
function worstCategory(entries: MonitorEntry[]): StatusCategory {
  let worst: StatusCategory = "success";
  for (const entry of entries) {
    const cat = categorizeStatusCode(entry.statusCode);
    if (SEVERITY[cat] > SEVERITY[worst]) {
      worst = cat;
    }
  }
  return worst;
}

export function bucketEntries(
  entries: MonitorEntry[],
  rangeStart: Date,
  rangeEnd: Date,
  segmentCount = 90
): Bucket[] {
  const duration = rangeEnd.getTime() - rangeStart.getTime();
  if (duration <= 0 || segmentCount <= 0) return [];
  const bucketMs = duration / segmentCount;

  const buckets: Bucket[] = Array.from({ length: segmentCount }, (_, i) => ({
    startTime: new Date(rangeStart.getTime() + i * bucketMs),
    endTime: new Date(rangeStart.getTime() + (i + 1) * bucketMs),
    entries: [],
    dominantCategory: "unknown" as StatusCategory,
  }));

  for (const entry of entries) {
    const t = new Date(entry.date).getTime();
    const idx = Math.floor((t - rangeStart.getTime()) / bucketMs);
    if (idx >= 0 && idx < segmentCount) {
      buckets[idx].entries.push(entry);
    }
  }

  for (const bucket of buckets) {
    if (bucket.entries.length === 0) continue;
    bucket.dominantCategory = worstCategory(bucket.entries);
  }

  return buckets;
}

export function calculateUptimePercent(entries: MonitorEntry[]): number {
  if (entries.length === 0) return 100;
  const up = entries.filter((e) => categorizeStatusCode(e.statusCode) === "success").length;
  return Math.round((up / entries.length) * 10000) / 100;
}
