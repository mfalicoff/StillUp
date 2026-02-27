<script lang="ts">
  import * as Card from "$lib/components/ui/card";
  import UptimeBar from "./UptimeBar.svelte";
  import StatusBadge from "./StatusBadge.svelte";
  import type { ServiceState } from "$lib/types";
  import { formatDistanceToNowStrict } from "date-fns";

  let { service, hours }: { service: ServiceState; hours: number } = $props();

  const rangeEnd = $derived(new Date());
  const rangeStart = $derived(new Date(Date.now() - hours * 60 * 60 * 1000));

  const lastCheckedAgo = $derived(
    service.lastChecked
      ? formatDistanceToNowStrict(new Date(service.lastChecked), { addSuffix: true })
      : null
  );

  const uptimeDisplay = $derived(
    service.entries.length === 0 ? "—" : `${service.uptimePercent}%`
  );

  const rangeLabel = $derived.by(() => {
    if (hours >= 168) return `${hours / 24}d`;
    if (hours >= 24) return "24h";
    return `${hours}h`;
  });
</script>

<Card.Root class="border-border">
  <Card.Content class="px-4 pb-3 pt-4">
    <!-- Header -->
    <div class="mb-3 flex items-start justify-between gap-3">
      <div class="min-w-0 flex-1">
        <div class="flex flex-wrap items-center gap-2">
          <span class="font-semibold tracking-tight truncate">{service.name}</span>
          <StatusBadge status={service.currentStatus} />
        </div>
        <a
          href={service.url}
          target="_blank"
          rel="noopener noreferrer"
          class="text-muted-foreground hover:text-foreground mt-0.5 block truncate text-xs transition-colors"
        >
          {service.url}
        </a>
      </div>
      <div class="shrink-0 text-right">
        <div class="text-lg font-semibold tabular-nums leading-none">{uptimeDisplay}</div>
        <div class="text-muted-foreground mt-0.5 text-xs">uptime</div>
      </div>
    </div>

    <!-- Uptime bar -->
    <UptimeBar entries={service.entries} {rangeStart} {rangeEnd} />

    <!-- Footer -->
    <div class="text-muted-foreground mt-1.5 flex justify-between text-xs">
      <span>{rangeLabel} ago</span>
      {#if lastCheckedAgo}
        <span>Checked {lastCheckedAgo}</span>
      {/if}
      <span>Now</span>
    </div>
  </Card.Content>
</Card.Root>
