import type { JobDetail, ApplicationStatus } from '$lib/types';

const CACHE_TTL = 5 * 60 * 1000; // 5 minutes

class JobsCache {
	private lastJobsFetch: number = 0;
	private jobDetailsCache: Record<string, { data: JobDetail; fetchedAt: number }> = {};

	shouldFetchJobs(hasJobs: boolean, force = false): boolean {
		if (force || !hasJobs) return true;
		return Date.now() - this.lastJobsFetch >= CACHE_TTL;
	}

	markJobsFetched(): void {
		this.lastJobsFetch = Date.now();
	}

	getCachedJobDetail(id: string, force = false): JobDetail | null {
		const cached = this.jobDetailsCache[id];
		if (!force && cached && Date.now() - cached.fetchedAt < CACHE_TTL) {
			return cached.data;
		}
		return null;
	}

	setJobDetail(id: string, data: JobDetail): void {
		const existing = this.jobDetailsCache[id];
		this.jobDetailsCache[id] = { 
			data, 
			fetchedAt: existing ? existing.fetchedAt : Date.now() 
		};
	}

	updateJobStatus(id: string, newStatus: ApplicationStatus): void {
		if (this.jobDetailsCache[id]) {
			this.jobDetailsCache[id].data.status = newStatus;
		}
	}

	deleteJobDetail(id: string): void {
		delete this.jobDetailsCache[id];
	}

	clear(): void {
		this.lastJobsFetch = 0;
		this.jobDetailsCache = {};
	}
}

export const jobsCache = new JobsCache();
