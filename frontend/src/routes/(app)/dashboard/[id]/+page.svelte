<script lang="ts">
    import { onMount } from 'svelte';
    import { page } from '$app/state';
    import { resolve } from '$app/paths';
    import { jobsStore } from '$lib/stores/jobs.svelte';
    import { Button } from '$lib/components/ui/button';
    import * as Card from '$lib/components/ui/card';
    import * as Select from '$lib/components/ui/select';
    import type { ApplicationStatus } from '$lib/types/jobs';

    const id = page.params.id;

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

    const triggerContent = $derived(
        statuses.find(s => s === selectedStatus) ?? 'Select status'
    );

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
    <p class="text-muted-foreground text-sm">Loading...</p>
{:else if jobsStore.error}
    <p class="text-destructive text-sm">{jobsStore.error}</p>
{:else if jobsStore.selectedJob}
    {@const job = jobsStore.selectedJob}
    <div class="flex flex-col gap-6 max-w-2xl">
        <div class="flex items-start justify-between">
            <div class="flex flex-col gap-1">
                <a
                    href={resolve('/dashboard')}
                    class="text-muted-foreground text-sm hover:underline"
                >
                    ← Back
                </a>
                <h1 class="text-2xl font-bold">{job.companyName}</h1>
                <p class="text-muted-foreground">{job.position}</p>
            </div>
            <Button variant="destructive" onclick={handleDelete}>
                Delete
            </Button>
        </div>

        <Card.Root>
            <Card.Header>
                <Card.Title>Details</Card.Title>
            </Card.Header>
            <Card.Content class="flex flex-col gap-4">
                <div class="flex items-center gap-3">
                    <span class="text-sm text-muted-foreground w-24">Status</span>
                    <Select.Root
                        type="single"
                        bind:value={selectedStatus}
                        onValueChange={handleStatusChange}
                    >
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
                        <span class="text-sm text-muted-foreground w-24">Job URL</span>
                        <a
                            href={job.jobUrl}
                            target="_blank"
                            rel="external noopener noreferrer"
                            class="text-primary text-sm underline-offset-4 hover:underline"
                        >
                            {job.jobUrl}
                        </a>
                    </div>
                {/if}

                {#if job.salaryMin}
                    <div class="flex items-center gap-3">
                        <span class="text-sm text-muted-foreground w-24">Salary</span>
                        <span class="text-sm">
                            {job.salaryMin.toLocaleString()} – {job.salaryMax?.toLocaleString()} {job.salaryCurrency}
                        </span>
                    </div>
                {/if}

                <div class="flex items-center gap-3">
                    <span class="text-sm text-muted-foreground w-24">Applied</span>
                    <span class="text-sm">{new Date(job.createdAt).toLocaleDateString()}</span>
                </div>
            </Card.Content>
        </Card.Root>
    </div>
{/if}