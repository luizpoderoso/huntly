<script lang="ts">
    import type { Snippet } from 'svelte';
    import * as Sidebar from '$lib/components/ui/sidebar';
    import AppSidebar from '$lib/components/app-sidebar.svelte';

    let { children }: { children: Snippet } = $props();
</script>

<Sidebar.Provider>
    <AppSidebar />
    <Sidebar.Inset>
        <header class="flex h-14 shrink-0 items-center gap-2 border-b px-4 md:hidden">
            <Sidebar.Trigger class="-ml-1" />
            <span class="font-semibold">Huntly</span>
        </header>
        <main class="p-8">
            {@render children()}
        </main>
    </Sidebar.Inset>
</Sidebar.Provider>