import type {
	JobSummary,
	JobDetail,
	CreateJobRequest,
	ApplicationStatus,
	AddInterviewRequest,
	RecordInterviewOutcomeRequest,
	ChangeInterviewNotesRequest,
	AddNoteRequest,
	ChangeNoteRequest
} from '$lib/types';
import * as jobsApi from '$lib/api';
import { toast } from 'svelte-sonner';

function createJobsStore() {
	let jobs = $state<JobSummary[]>([]);
	let selectedJob = $state<JobDetail | null>(null);
	let loading = $state(false);
	let error = $state<string | null>(null);

	return {
		get jobs() {
			return jobs;
		},
		get selectedJob() {
			return selectedJob;
		},
		get loading() {
			return loading;
		},
		get error() {
			return error;
		},

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
			try {
				const newJob = await jobsApi.createJob(request);
				jobs = [...jobs, newJob];
				toast.success('Job application created.');
				return newJob;
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to create job application.';
				toast.error(message);
				throw e;
			} finally {
				loading = false;
			}
		},

		async updateJobStatus(id: string, newStatus: ApplicationStatus) {
			try {
				await jobsApi.updateJobStatus(id, { newStatus });
				jobs = jobs.map((j) => (j.id === id ? { ...j, status: newStatus } : j));
				if (selectedJob?.id === id) selectedJob = { ...selectedJob, status: newStatus };
				toast.success('Status updated.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to update status.';
				toast.error(message);
				throw e;
			}
		},

		async deleteJob(id: string) {
			try {
				await jobsApi.deleteJob(id);
				jobs = jobs.filter((j) => j.id !== id);
				if (selectedJob?.id === id) selectedJob = null;
				toast.success('Job application deleted.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to delete job application.';
				toast.error(message);
				throw e;
			}
		},

		// Interviews
		async addInterview(jobId: string, request: AddInterviewRequest) {
			try {
				const interview = await jobsApi.addInterview(jobId, request);
				if (selectedJob?.id === jobId) {
					selectedJob = {
						...selectedJob,
						interviews: [...selectedJob.interviews, interview]
					};
				}
				toast.success('Interview added.');
				return interview;
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to add interview.';
				toast.error(message);
				throw e;
			}
		},

		async recordInterviewOutcome(
			jobId: string,
			interviewId: string,
			request: RecordInterviewOutcomeRequest
		) {
			try {
				await jobsApi.recordInterviewOutcome(jobId, interviewId, request);
				if (selectedJob?.id === jobId) {
					selectedJob = {
						...selectedJob,
						interviews: selectedJob.interviews.map((i) =>
							i.id === interviewId ? { ...i, outcome: request.newInterviewOutcome } : i
						)
					};
				}
				toast.success('Outcome recorded.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to record outcome.';
				toast.error(message);
				throw e;
			}
		},

		async changeInterviewNotes(
			jobId: string,
			interviewId: string,
			request: ChangeInterviewNotesRequest
		) {
			try {
				await jobsApi.changeInterviewNotes(jobId, interviewId, request);
				if (selectedJob?.id === jobId) {
					selectedJob = {
						...selectedJob,
						interviews: selectedJob.interviews.map((i) =>
							i.id === interviewId ? { ...i, notes: request.newInterviewNotes ?? null } : i
						)
					};
				}
				toast.success('Interview notes updated.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to update interview notes.';
				toast.error(message);
				throw e;
			}
		},

		async deleteInterview(jobId: string, interviewId: string) {
			try {
				await jobsApi.deleteInterview(jobId, interviewId);
				if (selectedJob?.id === jobId) {
					selectedJob = {
						...selectedJob,
						interviews: selectedJob.interviews.filter((i) => i.id !== interviewId)
					};
				}
				toast.success('Interview deleted.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to delete interview.';
				toast.error(message);
				throw e;
			}
		},

		// Notes
		async addNote(jobId: string, request: AddNoteRequest) {
			try {
				const note = await jobsApi.addNote(jobId, request);
				if (selectedJob?.id === jobId) {
					selectedJob = {
						...selectedJob,
						notes: [...selectedJob.notes, note]
					};
				}
				toast.success('Note added.');
				return note;
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to add note.';
				toast.error(message);
				throw e;
			}
		},

		async changeNote(jobId: string, noteId: string, request: ChangeNoteRequest) {
			try {
				await jobsApi.changeNote(jobId, noteId, request);
				if (selectedJob?.id === jobId) {
					selectedJob = {
						...selectedJob,
						notes: selectedJob.notes.map((n) =>
							n.id === noteId ? { ...n, content: request.newNoteContent } : n
						)
					};
				}
				toast.success('Note updated.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to update note.';
				toast.error(message);
				throw e;
			}
		},

		async deleteNote(jobId: string, noteId: string) {
			try {
				await jobsApi.deleteNote(jobId, noteId);
				if (selectedJob?.id === jobId) {
					selectedJob = {
						...selectedJob,
						notes: selectedJob.notes.filter((n) => n.id !== noteId)
					};
				}
				toast.success('Note deleted.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to delete note.';
				toast.error(message);
				throw e;
			}
		}
	};
}

export const jobsStore = createJobsStore();
