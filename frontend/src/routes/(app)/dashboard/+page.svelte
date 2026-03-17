<script lang="ts">
	import { onMount } from 'svelte';
	import { jobsStore } from '$lib/stores/jobs.svelte';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import * as Card from '$lib/components/ui/card';
	import * as Select from '$lib/components/ui/select';
	import JobCard from '$lib/components/jobs/JobCard.svelte';
	import CreateJobDialog from '$lib/components/jobs/CreateJobDialog.svelte';
	import { Skeleton } from '$lib/components/ui/skeleton';
	import type { ApplicationStatus } from '$lib/types';
	import StatsBar from '$lib/components/jobs/StatsBar.svelte';

	const statuses: (ApplicationStatus | 'All')[] = [
		'All',
		'Applied',
		'PhoneScreen',
		'TechnicalInterview',
		'OnsiteInterview',
		'Offer',
		'Rejected',
		'Withdrawn',
		'Ghosted'
	];

	const statusLabels: Record<ApplicationStatus | 'All', string> = {
		All: 'All statuses',
		Applied: 'Applied',
		PhoneScreen: 'Phone Screen',
		TechnicalInterview: 'Technical Interview',
		OnsiteInterview: 'Onsite Interview',
		Offer: 'Offer',
		Rejected: 'Rejected',
		Withdrawn: 'Withdrawn',
		Ghosted: 'Ghosted'
	};

	let createDialogOpen = $state(false);

	const filterTrigger = $derived(statusLabels[jobsStore.statusFilter]);

	onMount(async () => {
		await jobsStore.fetchJobs();
	});
</script>

<div class="flex flex-col gap-6">
	<div class="flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">Dashboard</h1>
			<p class="text-sm text-muted-foreground">
				{jobsStore.filteredJobs.length} of {jobsStore.jobs.length} application{jobsStore.jobs
					.length !== 1
					? 's'
					: ''}
			</p>
		</div>
		<Button onclick={() => (createDialogOpen = true)}>Add application</Button>
	</div>

	<StatsBar />

	<div class="flex gap-3">
		<Input
			placeholder="Search by company or position..."
			value={jobsStore.search}
			oninput={(e) => (jobsStore.search = e.currentTarget.value)}
			class="max-w-sm"
		/>
		<Select.Root
			type="single"
			value={jobsStore.statusFilter}
			onValueChange={(v) => (jobsStore.statusFilter = v as ApplicationStatus | 'All')}
		>
			<Select.Trigger class="w-48">
				{filterTrigger}
			</Select.Trigger>
			<Select.Content>
				{#each statuses as status (status)}
					<Select.Item value={status}>{statusLabels[status]}</Select.Item>
				{/each}
			</Select.Content>
		</Select.Root>
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
				<Button
					variant="outline"
					onclick={async () => {
						await jobsStore.seed();
					}}
				>
					Load sample data
				</Button>
			</Card.Content>
		</Card.Root>
	{:else if jobsStore.filteredJobs.length === 0}
		<Card.Root>
			<Card.Content class="flex flex-col items-center justify-center gap-3 py-12">
				<p class="text-muted-foreground">No applications match your search.</p>
				<Button
					variant="outline"
					onclick={() => {
						jobsStore.search = '';
						jobsStore.statusFilter = 'All';
					}}
				>
					Clear filters
				</Button>
			</Card.Content>
		</Card.Root>
	{:else}
		<div class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
			{#each jobsStore.filteredJobs as job (job.id)}
				<JobCard {job} />
			{/each}
		</div>
	{/if}
</div>

<CreateJobDialog bind:open={createDialogOpen} />
