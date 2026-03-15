<script lang="ts">
    import type { Snippet } from 'svelte';
    import * as Sidebar from '$lib/components/ui/sidebar';
    import AppSidebar from '$lib/components/app-sidebar.svelte';

    let { children }: { children: Snippet } = $props();
</script>

<Sidebar.Provider>
    <AppSidebar />
    <Sidebar.Inset>
        <main class="p-8">
            {@render children()}
        </main>
    </Sidebar.Inset>
</Sidebar.Provider>