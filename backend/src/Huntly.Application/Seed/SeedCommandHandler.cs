using Huntly.Application.Shared.Exceptions;
using Huntly.Application.Shared.Interfaces;
using Huntly.Core.Jobs.Entities;
using Huntly.Core.Jobs.Enums;
using Huntly.Core.Jobs.Repositories;
using Huntly.Core.Jobs.ValueObjects;
using MediatR;

namespace Huntly.Application.Seed;

public class SeedCommandHandler(
    IJobApplicationRepository repository,
    IAtomicWork atomicWork,
    IUserContext userContext)
    : IRequestHandler<SeedCommand>
{
    public async Task Handle(SeedCommand command, CancellationToken ct)
    {
        var count = await repository.CountByUserIdAsync(userContext.UserId, ct);

        if (count >= 5)
            throw new ConflictException("You already have data. Clear your applications before seeding.");

        var jobs = BuildSeedData(userContext.UserId);

        foreach (var job in jobs)
        {
            await repository.AddAsync(job, ct);

            foreach (var interview in job.Interviews)
                await repository.AddInterviewAsync(interview, ct);

            foreach (var note in job.Notes)
                await repository.AddNoteAsync(note, ct);
        }

        await atomicWork.CommitAsync(ct);
    }

    private static List<JobApplication> BuildSeedData(Guid userId)
    {
        var now = DateTime.UtcNow;
        var jobs = new List<JobApplication>();

        // 1. Offer received
        var job1 = JobApplication.Create(userId,
            new CompanyName("Stripe"),
            new Position("Senior Software Engineer"),
            new JobUrl("https://stripe.com/jobs"),
            new SalaryRange(120000, 160000, "USD"));
        job1.UpdateStatus(ApplicationStatus.Offer);
        job1.AddInterview(InterviewType.PhoneScreen, now.AddDays(-20), "Recruiter screen — 30 min", InterviewOutcome.Passed);
        job1.AddInterview(InterviewType.Technical, now.AddDays(-14), "System design + coding", InterviewOutcome.Passed);
        job1.AddInterview(InterviewType.Onsite, now.AddDays(-7), "4 rounds, full day", InterviewOutcome.Passed);
        job1.AddNote("Recruiter mentioned strong interest from the team.");
        job1.AddNote("Negotiating start date — they want mid-April.");
        jobs.Add(job1);

        // 2. Rejected after technical
        var job2 = JobApplication.Create(userId,
            new CompanyName("Airbnb"),
            new Position("Backend Engineer"),
            new JobUrl("https://careers.airbnb.com"),
            new SalaryRange(110000, 140000, "USD"));
        job2.UpdateStatus(ApplicationStatus.Rejected);
        job2.AddInterview(InterviewType.PhoneScreen, now.AddDays(-30), null, InterviewOutcome.Passed);
        job2.AddInterview(InterviewType.Technical, now.AddDays(-22), "LeetCode hard — struggled with graph problem", InterviewOutcome.Failed);
        job2.AddNote("Need to review graph algorithms before next application.");
        jobs.Add(job2);

        // 3. Onsite scheduled
        var job3 = JobApplication.Create(userId,
            new CompanyName("Linear"),
            new Position("Full-Stack Engineer"),
            new JobUrl("https://linear.app/careers"),
            new SalaryRange(100000, 130000, "USD"));
        job3.UpdateStatus(ApplicationStatus.OnsiteInterview);
        job3.AddInterview(InterviewType.PhoneScreen, now.AddDays(-15), null, InterviewOutcome.Passed);
        job3.AddInterview(InterviewType.Technical, now.AddDays(-8), "Went well — they loved the system design", InterviewOutcome.Passed);
        job3.AddInterview(InterviewType.Onsite, now.AddDays(3), "Onsite in SF — 5 rounds");
        job3.AddNote("Research their product roadmap before onsite.");
        jobs.Add(job3);

        // 4. Technical interview scheduled
        var job4 = JobApplication.Create(userId,
            new CompanyName("Vercel"),
            new Position("Software Engineer"),
            new JobUrl("https://vercel.com/careers"),
            null);
        job4.UpdateStatus(ApplicationStatus.TechnicalInterview);
        job4.AddInterview(InterviewType.PhoneScreen, now.AddDays(-10), "Quick 20 min call", InterviewOutcome.Passed);
        job4.AddInterview(InterviewType.Technical, now.AddDays(2), "Take-home then review call");
        jobs.Add(job4);

        // 5. Phone screen scheduled
        var job5 = JobApplication.Create(userId,
            new CompanyName("Notion"),
            new Position("Platform Engineer"),
            new JobUrl("https://notion.so/careers"),
            new SalaryRange(95000, 125000, "USD"));
        job5.UpdateStatus(ApplicationStatus.PhoneScreen);
        job5.AddInterview(InterviewType.PhoneScreen, now.AddDays(1), "HR intro call");
        job5.AddNote("They use a custom infra stack — research before the call.");
        jobs.Add(job5);

        // 6. Just applied
        var job6 = JobApplication.Create(userId,
            new CompanyName("PlanetScale"),
            new Position("Developer Advocate"),
            new JobUrl("https://planetscale.com/careers"),
            null);
        jobs.Add(job6);

        // 7. Ghosted
        var job7 = JobApplication.Create(userId,
            new CompanyName("Some Startup"),
            new Position("Backend Developer"),
            null, null);
        job7.UpdateStatus(ApplicationStatus.Ghosted);
        job7.AddNote("Applied 3 weeks ago — no response after follow-up email.");
        jobs.Add(job7);

        // 8. Withdrawn
        var job8 = JobApplication.Create(userId,
            new CompanyName("BigCorp Inc."),
            new Position("Software Engineer"),
            null,
            new SalaryRange(80000, 100000, "USD"));
        job8.UpdateStatus(ApplicationStatus.Withdrawn);
        job8.AddNote("Withdrew after learning about the on-call rotation requirements.");
        jobs.Add(job8);

        // 9. Applied
        var job9 = JobApplication.Create(userId,
            new CompanyName("Supabase"),
            new Position("Full-Stack Engineer"),
            new JobUrl("https://supabase.com/careers"),
            new SalaryRange(90000, 120000, "USD"));
        jobs.Add(job9);

        // 10. Applied with note
        var job10 = JobApplication.Create(userId,
            new CompanyName("Railway"),
            new Position("Software Engineer"),
            new JobUrl("https://railway.app/careers"),
            null);
        job10.AddNote("Small team, interesting infra challenges.");
        jobs.Add(job10);

        return jobs;
    }
}