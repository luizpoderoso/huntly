import { apiFetch } from './client';
import type { JobSummary, JobDetail, CreateJobRequest, UpdateJobStatusRequest } from '$lib/types';
import { authStore } from '$lib/stores';

function authHeader() {
    return { Authorization: `Bearer ${authStore.token}` };
}

export function getJobs() {
    return apiFetch<JobSummary[]>('/jobs', {
        headers: authHeader()
    });
}

export function getJob(id: string) {
    return apiFetch<JobDetail>(`/jobs/${id}`, {
        headers: authHeader()
    });
}

export function createJob(request: CreateJobRequest) {
    return apiFetch<JobSummary>('/jobs', {
        method: 'POST',
        body: JSON.stringify(request),
        headers: authHeader()
    });
}

export function updateJobStatus(id: string, request: UpdateJobStatusRequest) {
    return apiFetch<void>(`/jobs/${id}/status`, {
        method: 'PATCH',
        body: JSON.stringify(request),
        headers: authHeader()
    });
}

export function deleteJob(id: string) {
    return apiFetch<void>(`/jobs/${id}`, {
        method: 'DELETE',
        headers: authHeader()
    });
}