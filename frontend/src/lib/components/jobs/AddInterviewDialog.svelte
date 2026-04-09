<script lang="ts">
	import { jobsStore } from '$lib/stores';
	import type { InterviewType } from '$lib/types';
	import * as Dialog from '$lib/components/ui/dialog';
	import * as Select from '$lib/components/ui/select';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import { Textarea } from '$lib/components/ui/textarea';

	let { open = $bindable(), jobId }: { open: boolean; jobId: string } = $props();

	const interviewTypes: InterviewType[] = ['PhoneScreen', 'Technical', 'Onsite', 'Cultural', 'HR'];

	let selectedType = $state<InterviewType>('PhoneScreen');
	let scheduledAt = $state('');
	let notes = $state('');
	let errors = $state<Record<string, string[]> | null>(null);

	const triggerContent = $derived(interviewTypes.find((t) => t === selectedType) ?? 'Select type');

	function fieldError(field: string): string | null {
		return errors?.[field]?.[0] ?? null;
	}

	async function handleSubmit() {
		errors = null;
		try {
			await jobsStore.addInterview(jobId, {
				interviewType: selectedType,
				scheduledAt: new Date(scheduledAt).toISOString(),
				interviewNotes: notes || null
			});
			open = false;
			resetForm();
		} catch (e: unknown) {
			errors = (e as { errors: Record<string, string[]> })?.errors ?? null;
		}
	}

	function resetForm() {
		selectedType = 'PhoneScreen';
		scheduledAt = '';
		notes = '';
		errors = null;
	}
</script>

<Dialog.Root bind:open>
	<Dialog.Content class="sm:max-w-md">
		<Dialog.Header>
			<Dialog.Title>Add interview</Dialog.Title>
			<Dialog.Description>Schedule a new interview for this application.</Dialog.Description>
		</Dialog.Header>

		<form onsubmit={handleSubmit} class="flex flex-col gap-4">
			<div class="flex flex-col gap-2">
				<Label>Type</Label>
				<Select.Root type="single" bind:value={selectedType}>
					<Select.Trigger class="w-full hover:cursor-pointer">
						{triggerContent}
					</Select.Trigger>
					<Select.Content>
						{#each interviewTypes as type (type)}
							<Select.Item class="hover:cursor-pointer" value={type}>{type}</Select.Item>
						{/each}
					</Select.Content>
				</Select.Root>
				{#if fieldError('interviewType')}
					<p class="text-xs text-destructive">{fieldError('interviewType')}</p>
				{/if}
			</div>

			<div class="flex flex-col gap-2">
				<Label for="scheduledAt">Date & time</Label>
				<Input
					id="scheduledAt"
					type="datetime-local"
					class="hover:cursor-pointer"
					bind:value={scheduledAt}
				/>
				{#if fieldError('scheduledAt')}
					<p class="text-xs text-destructive">{fieldError('scheduledAt')}</p>
				{/if}
			</div>

			<div class="flex flex-col gap-2">
				<Label for="notes">Notes <span class="text-muted-foreground">(optional)</span></Label>
				<Textarea id="notes" bind:value={notes} placeholder="Any preparation notes..." rows={3} />
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
				<Button class="hover:cursor-pointer" type="submit" disabled={!scheduledAt}>
					Add interview
				</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>
