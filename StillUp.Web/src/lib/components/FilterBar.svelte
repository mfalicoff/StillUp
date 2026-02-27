<script lang="ts">
    import { Input } from "$lib/components/ui/input";
    import { Button } from "$lib/components/ui/button";
    import {
        monitorsState,
        initServices,
        TIME_RANGES,
        STATUS_FILTERS,
    } from "$lib/stores/monitors.svelte";
    import type { TimeRange } from "$lib/types";

    async function onTimeRangeChange(range: TimeRange) {
        if (monitorsState.filters.timeRange.hours === range.hours) return;
        monitorsState.filters.timeRange = range;
        monitorsState.loading = true;
        monitorsState.error = null;

        let resp: Response;
        try {
            resp = await fetch(`/api/monitors/recent/${range.hours}`);
        } catch (networkError) {
            console.error("[FilterBar] Network error fetching monitor data", {
                error: networkError,
                hours: range.hours,
            });
            monitorsState.loading = false;
            monitorsState.error =
                "Could not reach the server. Check your connection and try again.";
            return;
        }

        if (!resp.ok) {
            console.error("[FilterBar] Server error for monitor data request", {
                status: resp.status,
                hours: range.hours,
            });
            monitorsState.loading = false;
            monitorsState.error = `Failed to load monitor data (${resp.status}). Try again.`;
            return;
        }

        let data: {
            entries: Record<string, import("$lib/types").MonitorEntry[]>;
        };
        try {
            data = await resp.json();
        } catch (parseError) {
            console.error("[FilterBar] Failed to parse monitor data response", {
                error: parseError,
            });
            monitorsState.loading = false;
            monitorsState.error =
                "Received an unexpected response from the server. Try again.";
            return;
        }

        if (!data.entries) {
            console.error(
                "[FilterBar] Monitor data response missing 'entries' field",
                data,
            );
            monitorsState.loading = false;
            monitorsState.error =
                "Received incomplete data from the server. Try again.";
            return;
        }

        initServices(data.entries);
    }
</script>

<div class="mb-6 flex flex-wrap items-center gap-2">
    <!-- Search -->
    <Input
        placeholder="Filter services..."
        bind:value={monitorsState.filters.search}
        class="h-8 max-w-50 text-sm"
    />

    <!-- Time range -->
    <div class="flex gap-0.5 rounded-md border px-0.5 py-0.5">
        {#each TIME_RANGES as range (range.hours)}
            <Button
                variant={monitorsState.filters.timeRange.hours === range.hours
                    ? "default"
                    : "ghost"}
                size="sm"
                class="h-7 px-2.5 text-xs"
                onclick={() => onTimeRangeChange(range)}
            >
                {range.label}
            </Button>
        {/each}
    </div>

    <!-- Status filter -->
    <div class="flex gap-0.5 rounded-md border px-0.5 py-0.5">
        {#each STATUS_FILTERS as status (status)}
            <Button
                variant={monitorsState.filters.status === status
                    ? "default"
                    : "ghost"}
                size="sm"
                class="h-7 px-2.5 text-xs capitalize"
                onclick={() => (monitorsState.filters.status = status)}
            >
                {status}
            </Button>
        {/each}
    </div>

    <!-- SSE connection indicator -->
    <div
        class="ml-auto flex items-center gap-1.5 text-xs text-muted-foreground"
    >
        <span
            class="inline-block size-2 rounded-full {monitorsState.sseConnected
                ? 'bg-emerald-500'
                : 'bg-gray-400'}"
        ></span>
        <span>{monitorsState.sseConnected ? "Live" : "Connecting..."}</span>
    </div>
</div>
