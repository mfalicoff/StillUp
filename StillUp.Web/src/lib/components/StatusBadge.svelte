<script lang="ts">
  import { Badge } from "$lib/components/ui/badge";
  import type { ServiceStatus } from "$lib/types";

  let { status }: { status: ServiceStatus } = $props();

  const config: Record<ServiceStatus, { label: string; dot: string; badge: string }> = {
    up: { label: "Operational", dot: "bg-emerald-500", badge: "bg-emerald-500/15 text-emerald-700 dark:text-emerald-400 border-emerald-500/20" },
    degraded: { label: "Degraded", dot: "bg-yellow-500", badge: "bg-yellow-500/15 text-yellow-700 dark:text-yellow-400 border-yellow-500/20" },
    down: { label: "Down", dot: "bg-red-500", badge: "bg-red-500/15 text-red-700 dark:text-red-400 border-red-500/20" },
    unknown: { label: "Unknown", dot: "bg-gray-400", badge: "bg-gray-500/15 text-gray-600 dark:text-gray-400 border-gray-500/20" },
  };

  const current = $derived(config[status]);
</script>

<Badge variant="outline" class="text-xs font-medium {current.badge}">
  <span class="mr-1.5 inline-block size-1.5 rounded-full {current.dot}"></span>
  {current.label}
</Badge>
