<script lang="ts">
  import "../app.css";
  import { Separator } from "$lib/components/ui/separator";

  let { children } = $props();
</script>

<div class="min-h-screen bg-background">
  <header class="border-b">
    <div class="container mx-auto flex max-w-5xl items-center gap-3 px-4 py-4">
      <div class="flex items-center gap-2">
        <div class="flex size-7 items-center justify-center rounded-md bg-primary text-primary-foreground text-sm font-bold">
          S
        </div>
        <span class="text-lg font-semibold tracking-tight">StillUp</span>
      </div>
      <Separator orientation="vertical" class="h-5" />
      <span class="text-sm text-muted-foreground">Uptime Monitoring</span>
    </div>
  </header>

  {@render children()}
</div>
