<script lang="ts">
	import { jobsStore } from '$lib/stores';
	import * as Dialog from '$lib/components/ui/dialog';
	import { Button } from '$lib/components/ui/button';
	import { Label } from '$lib/components/ui/label';
	import { Textarea } from '$lib/components/ui/textarea';

	let { open = $bindable(), jobId }: { open: boolean; jobId: string } = $props();

	let content = $state('');
	let errors = $state<Record<string, string[]> | null>(null);

	function fieldError(field: string): string | null {
		return errors?.[field]?.[0] ?? null;
	}

	async function handleSubmit() {
		errors = null;
		try {
			await jobsStore.addNote(jobId, { noteContent: content });
			open = false;
			resetForm();
		} catch (e: unknown) {
			errors = (e as { errors: Record<string, string[]> })?.errors ?? null;
		}
	}

	function resetForm() {
		content = '';
		errors = null;
	}
</script>

<Dialog.Root bind:open>
	<Dialog.Content class="sm:max-w-md">
		<Dialog.Header>
			<Dialog.Title>Add note</Dialog.Title>
			<Dialog.Description>Add a note to this job application.</Dialog.Description>
		</Dialog.Header>

		<form onsubmit={handleSubmit} class="flex flex-col gap-4">
			<div class="flex flex-col gap-2">
				<Label for="content">Note</Label>
				<Textarea
					id="content"
					bind:value={content}
					placeholder="Write your note here..."
					rows={4}
				/>
				{#if fieldError('noteContent')}
					<p class="text-xs text-destructive">{fieldError('noteContent')}</p>
				{/if}
			</div>

			<Dialog.Footer>
				<Button
					class="hover:cursor-pointer"
					type="button"
					variant="outline"
					onclick={() => {
						open = false;
						resetForm();
					}}
				>
					Cancel
				</Button>
				<Button class="hover:cursor-pointer" type="submit" disabled={!content.trim()}
					>Add note</Button
				>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>
