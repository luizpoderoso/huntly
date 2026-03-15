<script lang="ts">
    import { resolve } from '$app/paths';
    import type { JobSummary } from '$lib/types/jobs';
    import * as Card from '$lib/components/ui/card';
    import { Badge } from '$lib/components/ui/badge';

    let { job }: { job: JobSummary } = $props();
</script>

<a href={resolve(`/dashboard/${job.id}`)} class="h-full">
    <Card.Root class="hover:bg-accent transition-colors cursor-pointer h-full flex flex-col">
        <Card.Header class="flex-1">
            <div class="flex items-start justify-between gap-2">
                <div class="flex flex-col gap-1">
                    <Card.Title class="text-base">{job.companyName}</Card.Title>
                    <Card.Description>{job.position}</Card.Description>
                </div>
                <Badge>{job.status}</Badge>
            </div>
        </Card.Header>
        <Card.Footer class="text-muted-foreground text-xs">
            Applied {new Date(job.createdAt).toLocaleDateString()}
        </Card.Footer>
    </Card.Root>
</a>