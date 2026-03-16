<script lang="ts">
	import { page } from "$app/state";
	import { jobsStore, type ApplicationStatus } from "$lib";
	import { InterviewCard, NoteCard, AddInterviewDialog, AddNoteDialog } from "$lib/components";
	import { Skeleton } from "$lib/components/ui/skeleton";
	import { resolve } from "$app/paths";
	import { onMount } from "svelte";
	import * as AlertDialog from "$lib/components/ui/alert-dialog";
	import * as Card from "$lib/components/ui/card";
	import * as Select from "$lib/components/ui/select";
	import * as Tabs from "$lib/components/ui/tabs";
	import { Button } from "$lib/components/ui/button";

	const id = page.params.id ?? '';

	const statuses: ApplicationStatus[] = [
		'Applied',
		'PhoneScreen',
		'TechnicalInterview',
		'OnsiteInterview',
		'Offer',
		'Rejected',
		'Withdrawn',
		'Ghosted'
	];

	let selectedStatus = $state<ApplicationStatus>('Applied');
	let addInterviewOpen = $state(false);
	let addNoteOpen = $state(false);

	const triggerContent = $derived(statuses.find((s) => s === selectedStatus) ?? 'Select status');

	onMount(async () => {
		if (!id) return;
		await jobsStore.fetchJob(id);
		if (jobsStore.selectedJob) {
			selectedStatus = jobsStore.selectedJob.status;
		}
	});

	async function handleStatusChange() {
		if (!id) return;
		await jobsStore.updateJobStatus(id, selectedStatus);
	}

	async function handleDelete() {
		if (!id) return;
		await jobsStore.deleteJob(id);
		window.location.href = resolve('/dashboard');
	}
</script>

{#if jobsStore.loading}
	<div class="flex max-w-2xl flex-col gap-6">
		<div class="flex items-start justify-between">
			<div class="flex flex-col gap-2">
				<Skeleton class="h-4 w-16" />
				<Skeleton class="h-8 w-48" />
				<Skeleton class="h-4 w-32" />
			</div>
			<Skeleton class="h-9 w-20" />
		</div>
		<div class="flex flex-col gap-4 rounded-lg border p-6">
			<Skeleton class="h-5 w-16" />
			{#each { length: 4 }, i (i)}
				<div class="flex items-center gap-3">
					<Skeleton class="h-4 w-24" />
					<Skeleton class="h-4 w-48" />
				</div>
			{/each}
		</div>
	</div>
{:else if jobsStore.error}
	<p class="text-sm text-destructive">{jobsStore.error}</p>
{:else if jobsStore.selectedJob}
	{@const job = jobsStore.selectedJob}
	<div class="flex max-w-2xl flex-col gap-6">
		<div class="flex items-start justify-between">
			<div class="flex flex-col gap-1">
				<a href={resolve('/dashboard')} class="text-sm text-muted-foreground hover:underline">
					← Back
				</a>
				<h1 class="text-2xl font-bold">{job.companyName}</h1>
				<p class="text-muted-foreground">{job.position}</p>
			</div>
			<AlertDialog.Root>
				<AlertDialog.Trigger>
					{#snippet child({ props })}
						<Button variant="destructive" {...props}>Delete</Button>
					{/snippet}
				</AlertDialog.Trigger>
				<AlertDialog.Content>
					<AlertDialog.Header>
						<AlertDialog.Title>Are you sure?</AlertDialog.Title>
						<AlertDialog.Description>
							This will permanently delete your application to <strong>{job.companyName}</strong>.
							This action cannot be undone.
						</AlertDialog.Description>
					</AlertDialog.Header>
					<AlertDialog.Footer>
						<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
						<AlertDialog.Action onclick={handleDelete}>Delete</AlertDialog.Action>
					</AlertDialog.Footer>
				</AlertDialog.Content>
			</AlertDialog.Root>
		</div>

		<Card.Root>
			<Card.Header>
				<Card.Title>Details</Card.Title>
			</Card.Header>
			<Card.Content class="flex flex-col gap-4">
				<div class="flex items-center gap-3">
					<span class="w-24 text-sm text-muted-foreground">Status</span>
					<Select.Root type="single" bind:value={selectedStatus} onValueChange={handleStatusChange}>
						<Select.Trigger class="w-48">
							{triggerContent}
						</Select.Trigger>
						<Select.Content>
							{#each statuses as status (status)}
								<Select.Item value={status}>{status}</Select.Item>
							{/each}
						</Select.Content>
					</Select.Root>
				</div>

				{#if job.jobUrl}
					<div class="flex items-center gap-3">
						<span class="w-24 text-sm text-muted-foreground">Job URL</span>
						<a
							href={job.jobUrl}
							target="_blank"
							rel="external noopener noreferrer"
							class="text-sm text-primary underline-offset-4 hover:underline"
						>
							{job.jobUrl}
						</a>
					</div>
				{/if}

				{#if job.salaryMin}
					<div class="flex items-center gap-3">
						<span class="w-24 text-sm text-muted-foreground">Salary</span>
						<span class="text-sm">
							{job.salaryMin.toLocaleString()} – {job.salaryMax?.toLocaleString()}
							{job.salaryCurrency}
						</span>
					</div>
				{/if}

				<div class="flex items-center gap-3">
					<span class="w-24 text-sm text-muted-foreground">Applied</span>
					<span class="text-sm">{new Date(job.createdAt).toLocaleDateString()}</span>
				</div>
			</Card.Content>
		</Card.Root>

		<Tabs.Root value="interviews">
			<Tabs.List class="w-full">
				<Tabs.Trigger value="interviews" class="flex-1">
					Interviews ({job.interviews.length})
				</Tabs.Trigger>
				<Tabs.Trigger value="notes" class="flex-1">
					Notes ({job.notes.length})
				</Tabs.Trigger>
			</Tabs.List>

			<Tabs.Content value="interviews" class="mt-4 flex flex-col gap-4">
				<div class="flex justify-end">
					<Button onclick={() => (addInterviewOpen = true)}>Add interview</Button>
				</div>
				{#if job.interviews.length === 0}
					<p class="py-6 text-center text-sm text-muted-foreground">No interviews yet.</p>
				{:else}
					{#each job.interviews as interview (interview.id)}
						<InterviewCard {interview} jobId={id} />
					{/each}
				{/if}
			</Tabs.Content>

			<Tabs.Content value="notes" class="mt-4 flex flex-col gap-4">
				<div class="flex justify-end">
					<Button onclick={() => (addNoteOpen = true)}>Add note</Button>
				</div>
				{#if job.notes.length === 0}
					<p class="py-6 text-center text-sm text-muted-foreground">No notes yet.</p>
				{:else}
					{#each job.notes as note (note.id)}
						<NoteCard {note} jobId={id} />
					{/each}
				{/if}
			</Tabs.Content>
		</Tabs.Root>
	</div>
{/if}

<AddInterviewDialog bind:open={addInterviewOpen} jobId={id} />
<AddNoteDialog bind:open={addNoteOpen} jobId={id} />
