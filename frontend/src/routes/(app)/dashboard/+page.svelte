<script lang="ts">
	import { onMount } from 'svelte';
	import { jobsStore } from '$lib/stores/jobs.svelte';
	import { Button } from '$lib/components/ui/button';
	import * as Card from '$lib/components/ui/card';
	import JobCard from '$lib/components/jobs/JobCard.svelte';
	import CreateJobDialog from '$lib/components/jobs/CreateJobDialog.svelte';
	import { Skeleton } from '$lib/components/ui/skeleton';

	let createDialogOpen = $state(false);

	onMount(async () => {
		await jobsStore.fetchJobs();
	});
</script>

<div class="flex flex-col gap-6">
	<div class="flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">Dashboard</h1>
			<p class="text-sm text-muted-foreground">
				{jobsStore.jobs.length} application{jobsStore.jobs.length !== 1 ? 's' : ''}
			</p>
		</div>
		<Button onclick={() => (createDialogOpen = true)}>Add application</Button>
	</div>

	{#if jobsStore.loading}
		<div class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
			{#each { length: 6 }, i (i)}
				<div class="flex flex-col gap-3 rounded-lg border p-6">
					<div class="flex justify-between">
						<Skeleton class="h-5 w-32" />
						<Skeleton class="h-5 w-20" />
					</div>
					<Skeleton class="h-4 w-48" />
					<Skeleton class="mt-2 h-3 w-24" />
				</div>
			{/each}
		</div>
	{:else if jobsStore.error}
		<p class="text-sm text-destructive">{jobsStore.error}</p>
	{:else if jobsStore.jobs.length === 0}
		<Card.Root>
			<Card.Content class="flex flex-col items-center justify-center gap-3 py-12">
				<p class="text-muted-foreground">No applications yet.</p>
				<Button onclick={() => (createDialogOpen = true)}>Add your first application</Button>
			</Card.Content>
		</Card.Root>
	{:else}
		<div class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
			{#each jobsStore.jobs as job (job.id)}
				<JobCard {job} />
			{/each}
		</div>
	{/if}
</div>

<CreateJobDialog bind:open={createDialogOpen} />
