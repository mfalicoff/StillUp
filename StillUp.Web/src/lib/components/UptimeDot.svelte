<script lang="ts">
  import * as Tooltip from "$lib/components/ui/tooltip";
  import { categoryToColorClass, categoryToLabel } from "$lib/utils/status";
  import type { Bucket } from "$lib/utils/uptime";
  import { format } from "date-fns";

  let { bucket }: { bucket: Bucket } = $props();

  const colorClass = $derived(categoryToColorClass(bucket.dominantCategory));

  const tooltipText = $derived.by(() => {
    const time = format(bucket.startTime, "MMM d, HH:mm");
    if (bucket.entries.length === 0) return `${time} — No data`;
    const label = categoryToLabel(bucket.dominantCategory);
    const count = bucket.entries.length;
    return `${time} — ${label} (${count} check${count > 1 ? "s" : ""})`;
  });
</script>

<Tooltip.Provider delayDuration={100}>
  <Tooltip.Root>
    <Tooltip.Trigger class="h-full flex-1 min-w-0">
      <div class="h-full w-full rounded-sm {colorClass} cursor-default transition-opacity hover:opacity-70"></div>
    </Tooltip.Trigger>
    <Tooltip.Content side="top">
      <p class="text-xs">{tooltipText}</p>
    </Tooltip.Content>
  </Tooltip.Root>
</Tooltip.Provider>
