<script lang="ts">
	import { jobsStore } from '$lib/stores';
	import * as Card from '$lib/components/ui/card';
	import { Progress } from '$lib/components/ui/progress';

	const stats = $derived.by(() => {
		const jobs = jobsStore.jobs;
		const total = jobs.length;

		const active = jobs.filter(
			(j) => !['Rejected', 'Withdrawn', 'Ghosted'].includes(j.status)
		).length;

		const interviewing = jobs.filter((j) =>
			['PhoneScreen', 'TechnicalInterview', 'OnsiteInterview'].includes(j.status)
		).length;

		const offers = jobs.filter((j) => j.status === 'Offer').length;

		const responded = jobs.filter((j) => j.status !== 'Applied').length;
		const responseRate = total > 0 ? Math.round((responded / total) * 100) : 0;

		return { total, active, interviewing, offers, responseRate };
	});
</script>

{#if jobsStore.jobs.length > 0}
	<div class="grid grid-cols-2 gap-4 md:grid-cols-4">
		<Card.Root class="overflow-hidden">
			<Card.Content class="pt-6">
				<p class="text-3xl font-bold">{stats.total}</p>
				<p class="mt-1 text-sm text-muted-foreground">Total</p>
			</Card.Content>
			<div class="h-1 w-full bg-blue-500"></div>
		</Card.Root>

		<Card.Root class="overflow-hidden">
			<Card.Content class="pt-6">
				<p class="text-3xl font-bold">{stats.active}</p>
				<p class="mt-1 text-sm text-muted-foreground">Active</p>
			</Card.Content>
			<div class="h-1 w-full bg-purple-500"></div>
		</Card.Root>

		<Card.Root class="overflow-hidden">
			<Card.Content class="pt-6">
				<p class="text-3xl font-bold">{stats.interviewing}</p>
				<p class="mt-1 text-sm text-muted-foreground">Interviewing</p>
			</Card.Content>
			<div class="h-1 w-full bg-yellow-500"></div>
		</Card.Root>

		<Card.Root class="overflow-hidden">
			<Card.Content class="pt-6">
				<p class="text-3xl font-bold">{stats.offers}</p>
				<p class="mt-1 text-sm text-muted-foreground">Offers</p>
			</Card.Content>
			<div class="h-1 w-full bg-green-500"></div>
		</Card.Root>

		<Card.Root class="col-span-2 overflow-hidden md:col-span-4">
			<Card.Content class="pt-6 pb-4">
				<div class="mb-2 flex items-center justify-between">
					<p class="text-sm text-muted-foreground">Response rate</p>
					<p class="text-sm font-bold">{stats.responseRate}%</p>
				</div>
				<Progress value={stats.responseRate} class="h-2" />
				<p class="mt-2 text-xs text-muted-foreground">
					{stats.responseRate > 0
						? `${jobsStore.jobs.filter((j) => j.status !== 'Applied').length} of ${stats.total} applications received a response`
						: 'No responses yet'}
				</p>
			</Card.Content>
			<div class="h-1 w-full bg-linear-to-r from-blue-500 via-purple-500 to-green-500"></div>
		</Card.Root>
	</div>
{/if}
