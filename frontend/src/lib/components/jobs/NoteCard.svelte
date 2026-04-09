<script lang="ts">
	import { jobsStore } from '$lib/stores';
	import type { NoteDto } from '$lib/types';
	import * as Card from '$lib/components/ui/card';
	import * as AlertDialog from '$lib/components/ui/alert-dialog';
	import { Button } from '$lib/components/ui/button';
	import { Textarea } from '$lib/components/ui/textarea';

	let { note, jobId }: { note: NoteDto; jobId: string } = $props();

	let editing = $state(false);
	// svelte-ignore state_referenced_locally
	let contentValue = $state(note.content);

	async function handleSave() {
		await jobsStore.changeNote(jobId, note.id, { newNoteContent: contentValue });
		editing = false;
	}

	function handleCancel() {
		contentValue = note.content;
		editing = false;
	}
</script>

<Card.Root>
	<Card.Header>
		<div class="flex items-start justify-between">
			<Card.Description>
				{new Date(note.createdAt).toLocaleDateString()}
			</Card.Description>
			<div class="flex gap-1">
				{#if !editing}
					<Button
						class="hover:cursor-pointer"
						variant="ghost"
						size="sm"
						onclick={() => (editing = true)}>Edit</Button
					>
				{/if}
				<AlertDialog.Root>
					<AlertDialog.Trigger>
						{#snippet child({ props })}
							<Button
								variant="ghost"
								size="sm"
								class="text-destructive hover:text-destructive"
								{...props}
							>
								Delete
							</Button>
						{/snippet}
					</AlertDialog.Trigger>
					<AlertDialog.Content>
						<AlertDialog.Header>
							<AlertDialog.Title>Delete note?</AlertDialog.Title>
							<AlertDialog.Description>
								This will permanently delete this note. This action cannot be undone.
							</AlertDialog.Description>
						</AlertDialog.Header>
						<AlertDialog.Footer>
							<AlertDialog.Cancel>Cancel</AlertDialog.Cancel>
							<AlertDialog.Action onclick={() => jobsStore.deleteNote(jobId, note.id)}>
								Delete
							</AlertDialog.Action>
						</AlertDialog.Footer>
					</AlertDialog.Content>
				</AlertDialog.Root>
			</div>
		</div>
	</Card.Header>

	<Card.Content>
		{#if editing}
			<div class="flex flex-col gap-2">
				<Textarea bind:value={contentValue} rows={3} />
				<div class="flex justify-end gap-2">
					<Button variant="outline" size="sm" onclick={handleCancel}>Cancel</Button>
					<Button size="sm" disabled={!contentValue.trim()} onclick={handleSave}>Save</Button>
				</div>
			</div>
		{:else}
			<p class="text-sm">{note.content}</p>
		{/if}
	</Card.Content>
</Card.Root>
