using Huntly.Core.Job.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huntly.Infra.Persistence.Context.Configurations.Jobs;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.OwnsOne(j => j.CompanyName, vo =>
            vo.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(200));

        builder.OwnsOne(j => j.Position, vo =>
            vo.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(200));

        builder.OwnsOne(j => j.JobUrl, vo =>
            vo.Property(x => x.Value)
                .HasMaxLength(2048));

        builder.OwnsOne(j => j.SalaryRange, vo =>
        {
            vo.Property(x => x.Min);
            vo.Property(x => x.Max);
            vo.Property(x => x.Currency).HasMaxLength(3);
        });

        builder.Property(j => j.Status)
            .HasConversion<string>()
            .IsRequired();
    }
}