<script lang="ts">
	import { jobsStore } from '$lib/stores';
	import type { InterviewDto, InterviewOutcome } from '$lib/types';
	import * as Card from '$lib/components/ui/card';
	import * as Select from '$lib/components/ui/select';
	import * as AlertDialog from '$lib/components/ui/alert-dialog';
	import { Button } from '$lib/components/ui/button';
	import { Textarea } from '$lib/components/ui/textarea';

	let { interview, jobId }: { interview: InterviewDto; jobId: string } = $props();

	const outcomes: InterviewOutcome[] = ['Pending', 'Passed', 'Failed', 'Cancelled'];

    // Props are stable per card instance — warning about initial $state values is safe to ignore
	// svelte-ignore state_referenced_locally
	let selectedOutcome = $state<InterviewOutcome>(interview.outcome);
	// svelte-ignore state_referenced_locally
	let notesValue = $state(interview.notes ?? '');

	let editingNotes = $state(false);

	const outcomeTrigger = $derived(outcomes.find((o) => o === selectedOutcome) ?? 'Select outcome');

	async function handleOutcomeChange() {
		if (selectedOutcome === interview.outcome) return;
		await jobsStore.recordInterviewOutcome(jobId, interview.id, {
			newInterviewOutcome: selectedOutcome
		});
	}

	async function handleSaveNotes() {
		await jobsStore.changeInterviewNotes(jobId, interview.id, {
			newInterviewNotes: notesValue || null
		});
		editingNotes = false;
	}

	function handleCancelNotes() {
		notesValue = interview.notes ?? '';
		editingNotes = false;
	}
</script>

<Card.Root>
	<Card.Header>
		<div class="flex items-start justify-between">
			<div class="flex flex-col gap-1">
				<Card.Title class="text-base">{interview.type}</Card.Title>
				<Card.Description>
					{new Date(interview.scheduledAt).toLocaleString()}
				</Card.Description>
			</div>
			<AlertDialog.Root>
				<AlertDialog.Trigger>
					{#snippet child({ props })}
						<Button
							variant="ghost"
							size="sm"
							class="text-destructive hover:text-destructive hover:cursor-pointer"
							{...props}
						>
							Delete
						</Button>
					{/snippet}
				</AlertDialog.Trigger>
				<AlertDialog.Content>
					<AlertDialog.Header>
						<AlertDialog.Title>Delete interview?</AlertDialog.Title>
						<AlertDialog.Description>
							This will permanently delete the {interview.type} interview. This action cannot be undone.
						</AlertDialog.Description>
					</AlertDialog.Header>
					<AlertDialog.Footer>
						<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
						<AlertDialog.Action onclick={() => jobsStore.deleteInterview(jobId, interview.id)}>
							Delete
						</AlertDialog.Action>
					</AlertDialog.Footer>
				</AlertDialog.Content>
			</AlertDialog.Root>
		</div>
	</Card.Header>

	<Card.Content class="flex flex-col gap-4">
		<div class="flex items-center gap-3">
			<span class="w-20 text-sm text-muted-foreground">Outcome</span>
			<Select.Root
				type="single"
				bind:value={selectedOutcome}
				onValueChange={handleOutcomeChange}
				disabled={interview.outcome !== 'Pending'}
			>
				<Select.Trigger class="w-40 hover:cursor-pointer">
					{outcomeTrigger}
				</Select.Trigger>
				<Select.Content>
					{#each outcomes as outcome (outcome)}
						<Select.Item class="hover:cursor-pointer" value={outcome}>{outcome}</Select.Item>
					{/each}
				</Select.Content>
			</Select.Root>
		</div>

		<div class="flex flex-col gap-2">
			<div class="flex items-center justify-between">
				<span class="text-sm text-muted-foreground">Notes</span>
				{#if !editingNotes}
					<Button variant="ghost" size="sm" class="hover:cursor-pointer" onclick={() => (editingNotes = true)}>Edit</Button>
				{/if}
			</div>
			{#if editingNotes}
				<Textarea bind:value={notesValue} rows={3} />
				<div class="flex justify-end gap-2">
					<Button variant="outline" size="sm" onclick={handleCancelNotes}>Cancel</Button>
					<Button size="sm" onclick={handleSaveNotes}>Save</Button>
				</div>
			{:else}
				<p class="text-sm text-muted-foreground">
					{interview.notes ?? 'No notes.'}
				</p>
			{/if}
		</div>
	</Card.Content>
</Card.Root>
