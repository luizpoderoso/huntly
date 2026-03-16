<script lang="ts">
    import type { ApplicationStatus } from '$lib/types';
    import { Badge } from '$lib/components/ui/badge';

    let { status }: { status: ApplicationStatus } = $props();

    const variantMap: Record<ApplicationStatus, string> = {
        Applied: 'bg-blue-100 text-blue-800 hover:bg-blue-100',
        PhoneScreen: 'bg-purple-100 text-purple-800 hover:bg-purple-100',
        TechnicalInterview: 'bg-yellow-100 text-yellow-800 hover:bg-yellow-100',
        OnsiteInterview: 'bg-orange-100 text-orange-800 hover:bg-orange-100',
        Offer: 'bg-green-100 text-green-800 hover:bg-green-100',
        Rejected: 'bg-red-100 text-red-800 hover:bg-red-100',
        Withdrawn: 'bg-gray-100 text-gray-600 hover:bg-gray-100',
        Ghosted: 'bg-gray-100 text-gray-400 hover:bg-gray-100'
    };

    const labelMap: Record<ApplicationStatus, string> = {
        Applied: 'Applied',
        PhoneScreen: 'Phone Screen',
        TechnicalInterview: 'Technical',
        OnsiteInterview: 'Onsite',
        Offer: 'Offer',
        Rejected: 'Rejected',
        Withdrawn: 'Withdrawn',
        Ghosted: 'Ghosted'
    };
</script>

<Badge class={variantMap[status]}>{labelMap[status]}</Badge>