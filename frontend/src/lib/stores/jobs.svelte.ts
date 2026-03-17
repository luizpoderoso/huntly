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
import { jobsCache } from '$lib/caches/jobs.cache';

function createJobsStore() {
	let jobs = $state<JobSummary[]>([]);
	let selectedJob = $state<JobDetail | null>(null);

	let loading = $state(false);
	let error = $state<string | null>(null);
	let search = $state('');
	let statusFilter = $state<ApplicationStatus | 'All'>('All');

	function syncSelectedJob() {
		if (selectedJob) {
			jobsCache.setJobDetail(selectedJob.id, selectedJob);
		}
	}

	// Automatically recomputes when jobs, search, or statusFilter changes
	const filteredJobs = $derived(
		jobs.filter((j) => {
			const matchesSearch =
				search === '' ||
				j.companyName.toLowerCase().includes(search.toLowerCase()) ||
				j.position.toLowerCase().includes(search.toLowerCase());

			const matchesStatus = statusFilter === 'All' || j.status === statusFilter;

			return matchesSearch && matchesStatus;
		})
	);

	return {
		get jobs() {
			return jobs;
		},
		get filteredJobs() {
			return filteredJobs;
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
		get search() {
			return search;
		},
		get statusFilter() {
			return statusFilter;
		},
		set search(value: string) {
			search = value;
		},
		set statusFilter(value: ApplicationStatus | 'All') {
			statusFilter = value;
		},

		async fetchJobs(force = false) {
			if (!jobsCache.shouldFetchJobs(jobs.length > 0, force)) {
				return;
			}
			loading = true;
			error = null;
			try {
				jobs = await jobsApi.getJobs();
				jobsCache.markJobsFetched();
			} catch (e: unknown) {
				error = (e as { error: string })?.error ?? 'Failed to load jobs.';
			} finally {
				loading = false;
			}
		},

		async fetchJob(id: string, force = false) {
			const cached = jobsCache.getCachedJobDetail(id, force);
			if (cached) {
				selectedJob = cached;
				return;
			}
			loading = true;
			error = null;
			try {
				selectedJob = await jobsApi.getJob(id);
				jobsCache.setJobDetail(id, selectedJob);
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
				if (selectedJob?.id === id) {
					selectedJob = { ...selectedJob, status: newStatus };
					syncSelectedJob();
				} else {
					jobsCache.updateJobStatus(id, newStatus);
				}
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
				jobsCache.deleteJobDetail(id);
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
					syncSelectedJob();
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
					syncSelectedJob();
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
					syncSelectedJob();
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
					syncSelectedJob();
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
					syncSelectedJob();
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
					syncSelectedJob();
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
					syncSelectedJob();
				}
				toast.success('Note deleted.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to delete note.';
				toast.error(message);
				throw e;
			}
		},

		async seed() {
			loading = true;
			try {
				await jobsApi.seedJobs();
				jobsCache.clear();
				await this.fetchJobs(true);
				toast.success('Sample data loaded successfully.');
			} catch (e: unknown) {
				const message = (e as { error: string })?.error ?? 'Failed to load sample data.';
				toast.error(message);
				throw e;
			} finally {
				loading = false;
			}
		}
	};
}

export const jobsStore = createJobsStore();
