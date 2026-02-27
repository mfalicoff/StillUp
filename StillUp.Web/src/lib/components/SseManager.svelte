<script lang="ts">
  import { onMount } from "svelte";
  import { monitorsState, upsertEntry } from "$lib/stores/monitors.svelte";
  import type { MonitorEntry } from "$lib/types";

  onMount(() => {
    let es: EventSource;
    let reconnectTimer: ReturnType<typeof setTimeout>;
    let retryDelay = 1000;
    const MAX_DELAY = 30_000;

    function connect() {
      es = new EventSource("/api/monitors/stream");

      es.addEventListener("open", () => {
        monitorsState.sseConnected = true;
        retryDelay = 1000;
      });

      es.addEventListener("monitor-entry", (event) => {
        let entry: MonitorEntry;
        try {
          entry = JSON.parse(event.data) as MonitorEntry;
        } catch (parseError) {
          console.error("[SSE] Failed to parse monitor-entry JSON", { error: parseError, rawData: event.data });
          return;
        }
        if (!entry.name || !entry.url || !entry.date) {
          console.error("[SSE] Received structurally invalid monitor-entry", entry);
          return;
        }
        try {
          upsertEntry(entry);
        } catch (storeError) {
          console.error("[SSE] Failed to upsert monitor entry into store", { error: storeError, entry });
        }
      });

      es.addEventListener("error", () => {
        es.close();
        monitorsState.sseConnected = false;
        console.error("[SSE] Connection error, scheduling reconnect", {
          retryCount: retryDelay,
          retryDelayMs: retryDelay,
          readyState: es.readyState,
        });
        retryDelay = Math.min(retryDelay * 2, MAX_DELAY);
        reconnectTimer = setTimeout(() => {
          connect();
        }, retryDelay);
      });
    }

    connect();

    return () => {
      clearTimeout(reconnectTimer);
      es?.close();
      monitorsState.sseConnected = false;
    };
  });
</script>
