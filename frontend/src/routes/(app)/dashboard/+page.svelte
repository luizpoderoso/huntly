<script lang="ts">
    import { onMount } from 'svelte';
    import { jobsStore } from '$lib/stores/jobs.svelte';
    import { Button } from '$lib/components/ui/button';
    import * as Card from '$lib/components/ui/card';
    import JobCard from '$lib/components/jobs/JobCard.svelte';
    import CreateJobDialog from '$lib/components/jobs/CreateJobDialog.svelte';

    let createDialogOpen = $state(false);

    onMount(async () => {
        await jobsStore.fetchJobs();
    });
</script>

<div class="flex flex-col gap-6">
    <div class="flex items-center justify-between">
        <div>
            <h1 class="text-2xl font-bold">Dashboard</h1>
            <p class="text-muted-foreground text-sm">
                {jobsStore.jobs.length} application{jobsStore.jobs.length !== 1 ? 's' : ''}
            </p>
        </div>
        <Button onclick={() => createDialogOpen = true}>
            Add application
        </Button>
    </div>

    {#if jobsStore.loading}
        <p class="text-muted-foreground text-sm">Loading...</p>
    {:else if jobsStore.error}
        <p class="text-destructive text-sm">{jobsStore.error}</p>
    {:else if jobsStore.jobs.length === 0}
        <Card.Root>
            <Card.Content class="flex flex-col items-center justify-center py-12 gap-3">
                <p class="text-muted-foreground">No applications yet.</p>
                <Button onclick={() => createDialogOpen = true}>
                    Add your first application
                </Button>
            </Card.Content>
        </Card.Root>
    {:else}
        <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
            {#each jobsStore.jobs as job (job.id)}
                <JobCard {job} />
            {/each}
        </div>
    {/if}
</div>

<CreateJobDialog bind:open={createDialogOpen} />