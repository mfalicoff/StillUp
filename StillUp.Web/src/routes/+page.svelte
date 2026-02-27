<script lang="ts">
  import { onMount } from "svelte";
  import { Skeleton } from "$lib/components/ui/skeleton";
  import { monitorsState, initServices, getFilteredServices } from "$lib/stores/monitors.svelte";
  import FilterBar from "$lib/components/FilterBar.svelte";
  import MonitorCard from "$lib/components/MonitorCard.svelte";
  import SseManager from "$lib/components/SseManager.svelte";
  import type { PageData } from "./$types";

  let { data }: { data: PageData } = $props();

  onMount(() => {
    initServices(data.initialData.entries);
  });

  const filteredServices = $derived(getFilteredServices());
</script>

<SseManager />

<main class="container mx-auto max-w-5xl px-4 py-8">
  <div class="mb-6">
    <h1 class="text-2xl font-bold tracking-tight">Services</h1>
    <p class="text-muted-foreground mt-1 text-sm">
      Real-time uptime monitoring for your Docker services
    </p>
  </div>

  <FilterBar />

  {#if monitorsState.loading}
    <div class="space-y-3">
      {#each { length: 3 } as _}
        <div class="rounded-lg border p-4 space-y-3">
          <div class="flex justify-between items-start">
            <div class="space-y-2 flex-1">
              <Skeleton class="h-4 w-32" />
              <Skeleton class="h-3 w-48" />
            </div>
            <Skeleton class="h-8 w-12" />
          </div>
          <Skeleton class="h-8 w-full" />
          <div class="flex justify-between">
            <Skeleton class="h-3 w-8" />
            <Skeleton class="h-3 w-20" />
            <Skeleton class="h-3 w-8" />
          </div>
        </div>
      {/each}
    </div>
  {:else if monitorsState.error}
    <div class="rounded-lg border border-destructive/20 bg-destructive/5 p-6 text-center">
      <p class="text-sm font-medium text-destructive">Failed to load monitor data</p>
      <p class="mt-1 text-xs text-muted-foreground">{monitorsState.error}</p>
    </div>
  {:else if filteredServices.length === 0}
    <div class="py-20 text-center">
      {#if Object.keys(monitorsState.services).length === 0}
        <p class="text-muted-foreground text-sm">
          No services found. Make sure your Docker containers have the
          <code class="rounded bg-muted px-1 py-0.5 text-xs">stillup.name</code> and
          <code class="rounded bg-muted px-1 py-0.5 text-xs">stillup.url</code> labels.
        </p>
      {:else}
        <p class="text-muted-foreground text-sm">No services match the current filters.</p>
      {/if}
    </div>
  {:else}
    <div class="space-y-3">
      {#each filteredServices as service (service.name)}
        <MonitorCard {service} hours={monitorsState.filters.timeRange.hours} />
      {/each}
    </div>

    <p class="mt-4 text-center text-xs text-muted-foreground">
      {filteredServices.length} service{filteredServices.length !== 1 ? "s" : ""} monitored
    </p>
  {/if}
</main>
