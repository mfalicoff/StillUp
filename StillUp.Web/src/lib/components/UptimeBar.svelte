<script lang="ts">
  import UptimeDot from "./UptimeDot.svelte";
  import { bucketEntries } from "$lib/utils/uptime";
  import type { MonitorEntry } from "$lib/types";

  let {
    entries,
    rangeStart,
    rangeEnd,
    segments = 90,
  }: {
    entries: MonitorEntry[];
    rangeStart: Date;
    rangeEnd: Date;
    segments?: number;
  } = $props();

  const buckets = $derived(bucketEntries(entries, rangeStart, rangeEnd, segments));
</script>

<div class="flex h-8 w-full gap-px">
  {#each buckets as bucket, i (i)}
    <UptimeDot {bucket} />
  {/each}
</div>
