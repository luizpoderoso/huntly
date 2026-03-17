import { apiFetch } from './client';
import type {
	JobSummary,
	JobDetail,
	InterviewDto,
	NoteDto,
	CreateJobRequest,
	UpdateJobStatusRequest,
	AddInterviewRequest,
	RecordInterviewOutcomeRequest,
	ChangeInterviewNotesRequest,
	AddNoteRequest,
	ChangeNoteRequest
} from '$lib/types';
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

// Interviews
export function addInterview(jobId: string, request: AddInterviewRequest) {
	return apiFetch<InterviewDto>(`/jobs/${jobId}/interviews`, {
		method: 'POST',
		body: JSON.stringify(request),
		headers: authHeader()
	});
}

export function recordInterviewOutcome(
	jobId: string,
	interviewId: string,
	request: RecordInterviewOutcomeRequest
) {
	return apiFetch<void>(`/jobs/${jobId}/interviews/${interviewId}/outcome`, {
		method: 'PATCH',
		body: JSON.stringify(request),
		headers: authHeader()
	});
}

export function changeInterviewNotes(
	jobId: string,
	interviewId: string,
	request: ChangeInterviewNotesRequest
) {
	return apiFetch<void>(`/jobs/${jobId}/interviews/${interviewId}/notes`, {
		method: 'PATCH',
		body: JSON.stringify(request),
		headers: authHeader()
	});
}

export function deleteInterview(jobId: string, interviewId: string) {
	return apiFetch<void>(`/jobs/${jobId}/interviews/${interviewId}`, {
		method: 'DELETE',
		headers: authHeader()
	});
}

// Notes
export function addNote(jobId: string, request: AddNoteRequest) {
	return apiFetch<NoteDto>(`/jobs/${jobId}/notes`, {
		method: 'POST',
		body: JSON.stringify(request),
		headers: authHeader()
	});
}

export function changeNote(jobId: string, noteId: string, request: ChangeNoteRequest) {
	return apiFetch<void>(`/jobs/${jobId}/notes/${noteId}`, {
		method: 'PATCH',
		body: JSON.stringify(request),
		headers: authHeader()
	});
}

export function deleteNote(jobId: string, noteId: string) {
	return apiFetch<void>(`/jobs/${jobId}/notes/${noteId}`, {
		method: 'DELETE',
		headers: authHeader()
	});
}

export function seedJobs() {
    return apiFetch<void>('/seed', {
        method: 'POST',
        headers: authHeader()
    });
}