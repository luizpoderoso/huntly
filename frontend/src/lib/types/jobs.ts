export interface JobSummary {
    id: string;
    companyName: string;
    position: string;
    status: ApplicationStatus;
    jobUrl: string | null;
    createdAt: string;
    updatedAt: string;
}

export interface JobDetail extends JobSummary {
    salaryMin: number | null;
    salaryMax: number | null;
    salaryCurrency: string | null;
    interviews: InterviewDto[];
    notes: NoteDto[];
}

export interface InterviewDto {
    id: string;
    type: InterviewType;
    scheduledAt: string;
    outcome: InterviewOutcome;
    notes: string | null;
}

export interface NoteDto {
    id: string;
    content: string;
    createdAt: string;
}

export interface CreateJobRequest {
    companyName: string;
    position: string;
    jobUrl?: string;
    salaryMin?: number;
    salaryMax?: number;
    salaryCurrency?: string;
}

export interface UpdateJobStatusRequest {
    newStatus: ApplicationStatus;
}

export interface AddInterviewRequest {
    interviewType: InterviewType;
    scheduledAt: string;
    interviewNotes?: string | null;
}

export interface RecordInterviewOutcomeRequest {
    newInterviewOutcome: InterviewOutcome;
}

export interface ChangeInterviewNotesRequest {
    newInterviewNotes?: string | null;
}

export interface AddNoteRequest {
    noteContent: string;
}

export interface ChangeNoteRequest {
    newNoteContent: string;
}

export type ApplicationStatus =
    | 'Applied'
    | 'PhoneScreen'
    | 'TechnicalInterview'
    | 'OnsiteInterview'
    | 'Offer'
    | 'Rejected'
    | 'Withdrawn'
    | 'Ghosted';

export type InterviewType =
    | 'PhoneScreen'
    | 'Technical'
    | 'Onsite'
    | 'Cultural'
    | 'HR';

export type InterviewOutcome =
    | 'Pending'
    | 'Passed'
    | 'Failed'
    | 'Cancelled';