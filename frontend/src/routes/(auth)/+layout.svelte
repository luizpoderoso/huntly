<script lang="ts">
    import type { Snippet } from 'svelte';

    let { children }: { children: Snippet } = $props();
</script>

<main class="min-h-screen flex items-center justify-center bg-background">
    <div class="w-full max-w-md px-4">
        {@render children()}
    </div>
</main>