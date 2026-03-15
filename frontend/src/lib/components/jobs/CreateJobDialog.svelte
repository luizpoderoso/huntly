<script lang="ts">
    import { jobsStore } from '$lib/stores/jobs.svelte';
    import type { CreateJobRequest } from '$lib/types/jobs';
    import * as Dialog from '$lib/components/ui/dialog';
    import { Button } from '$lib/components/ui/button';
    import { Input } from '$lib/components/ui/input';
    import { Label } from '$lib/components/ui/label';

    let { open = $bindable() }: { open: boolean } = $props();

    let companyName = $state('');
    let position = $state('');
    let jobUrl = $state('');
    let salaryMin = $state('');
    let salaryMax = $state('');
    let salaryCurrency = $state('');
    let errors = $state<Record<string, string[]> | null>(null);

    function fieldError(field: string): string | null {
        return errors?.[field]?.[0] ?? null;
    }

    async function handleSubmit() {
        errors = null;
        try {
            const request: CreateJobRequest = {
                companyName,
                position,
                jobUrl: jobUrl || undefined,
                salaryMin: salaryMin ? Number(salaryMin) : undefined,
                salaryMax: salaryMax ? Number(salaryMax) : undefined,
                salaryCurrency: salaryCurrency || undefined
            };

            await jobsStore.createJob(request);
            open = false;
            resetForm();
        } catch (e: unknown) {
            errors = (e as { errors: Record<string, string[]> })?.errors ?? null;
        }
    }

    function resetForm() {
        companyName = '';
        position = '';
        jobUrl = '';
        salaryMin = '';
        salaryMax = '';
        salaryCurrency = '';
        errors = null;
    }
</script>

<Dialog.Root bind:open>
    <Dialog.Content class="sm:max-w-md">
        <Dialog.Header>
            <Dialog.Title>Add application</Dialog.Title>
            <Dialog.Description>Track a new job application.</Dialog.Description>
        </Dialog.Header>

        <form onsubmit={handleSubmit} class="flex flex-col gap-4">
            <div class="flex flex-col gap-2">
                <Label for="companyName">Company name *</Label>
                <Input
                    id="companyName"
                    bind:value={companyName}
                    placeholder="Acme Corp"
                />
                {#if fieldError('companyName')}
                    <p class="text-destructive text-xs">{fieldError('companyName')}</p>
                {/if}
            </div>

            <div class="flex flex-col gap-2">
                <Label for="position">Position *</Label>
                <Input
                    id="position"
                    bind:value={position}
                    placeholder="Software Engineer"
                />
                {#if fieldError('position')}
                    <p class="text-destructive text-xs">{fieldError('position')}</p>
                {/if}
            </div>

            <div class="flex flex-col gap-2">
                <Label for="jobUrl">Job URL</Label>
                <Input
                    id="jobUrl"
                    bind:value={jobUrl}
                    placeholder="https://..."
                />
                {#if fieldError('jobUrl')}
                    <p class="text-destructive text-xs">{fieldError('jobUrl')}</p>
                {/if}
            </div>

            <div class="grid grid-cols-3 gap-2">
                <div class="flex flex-col gap-2">
                    <Label for="salaryMin">Min salary</Label>
                    <Input
                        id="salaryMin"
                        type="number"
                        bind:value={salaryMin}
                        placeholder="50000"
                    />
                </div>
                <div class="flex flex-col gap-2">
                    <Label for="salaryMax">Max salary</Label>
                    <Input
                        id="salaryMax"
                        type="number"
                        bind:value={salaryMax}
                        placeholder="80000"
                    />
                </div>
                <div class="flex flex-col gap-2">
                    <Label for="salaryCurrency">Currency</Label>
                    <Input
                        id="salaryCurrency"
                        bind:value={salaryCurrency}
                        placeholder="USD"
                    />
                </div>
            </div>
            {#if fieldError('salaryMin') || fieldError('salaryMax') || fieldError('salaryCurrency')}
                <p class="text-destructive text-xs">
                    {fieldError('salaryMin') ?? fieldError('salaryMax') ?? fieldError('salaryCurrency')}
                </p>
            {/if}

            <Dialog.Footer>
                <Button type="button" variant="outline" onclick={() => { open = false; resetForm(); }}>
                    Cancel
                </Button>
                <Button type="submit" disabled={jobsStore.loading}>
                    {jobsStore.loading ? 'Creating...' : 'Add application'}
                </Button>
            </Dialog.Footer>
        </form>
    </Dialog.Content>
</Dialog.Root>