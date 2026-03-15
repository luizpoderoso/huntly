<script lang="ts">
    import type { Snippet } from 'svelte';
    import './layout.css';
	import { Toaster } from '$lib/components/ui/sonner';

    let { children }: { children: Snippet } = $props();
</script>

{@render children()}
<Toaster />