import type { JobSummary, JobDetail, CreateJobRequest, ApplicationStatus } from '$lib/types';
import * as jobsApi from '$lib/api';

function createJobsStore() {
    let jobs = $state<JobSummary[]>([]);
    let selectedJob = $state<JobDetail | null>(null);
    let loading = $state(false);
    let error = $state<string | null>(null);

    return {
        get jobs() { return jobs; },
        get selectedJob() { return selectedJob; },
        get loading() { return loading; },
        get error() { return error; },

        async fetchJobs() {
            loading = true;
            error = null;
            try {
                jobs = await jobsApi.getJobs();
            } catch (e: unknown) {
                error = (e as { error: string })?.error ?? 'Failed to load jobs.';
            } finally {
                loading = false;
            }
        },

        async fetchJob(id: string) {
            loading = true;
            error = null;
            try {
                selectedJob = await jobsApi.getJob(id);
            } catch (e: unknown) {
                error = (e as { error: string })?.error ?? 'Failed to load job.';
            } finally {
                loading = false;
            }
        },

        async createJob(request: CreateJobRequest) {
            loading = true;
            error = null;
            try {
                const id = await jobsApi.createJob(request);
                await this.fetchJobs();
                return id;
            } catch (e: unknown) {
                error = (e as { error: string })?.error ?? 'Failed to create job.';
                throw e;
            } finally {
                loading = false;
            }
        },

        async updateJobStatus(id: string, newStatus: ApplicationStatus) {
            error = null;
            try {
                await jobsApi.updateJobStatus(id, { newStatus });
                // Optimistic update — no need to refetch the whole list
                jobs = jobs.map(j =>
                    j.id === id ? { ...j, status: newStatus } : j
                );
                if (selectedJob?.id === id) {
                    selectedJob = { ...selectedJob, status: newStatus };
                }
            } catch (e: unknown) {
                error = (e as { error: string })?.error ?? 'Failed to update status.';
                throw e;
            }
        },

        async deleteJob(id: string) {
            error = null;
            try {
                await jobsApi.deleteJob(id);
                jobs = jobs.filter(j => j.id !== id);
                if (selectedJob?.id === id) selectedJob = null;
            } catch (e: unknown) {
                error = (e as { error: string })?.error ?? 'Failed to delete job.';
                throw e;
            }
        }
    };
}

export const jobsStore = createJobsStore();