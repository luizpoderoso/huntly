using Huntly.Core.Jobs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huntly.Infra.Persistence.Context.Configurations.Jobs;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.OwnsOne(j => j.CompanyName, vo =>
            vo.Property(x => x.Value)
                .HasColumnName("company_name")
                .HasMaxLength(200));

        builder.OwnsOne(j => j.Position, vo =>
            vo.Property(x => x.Value)
                .HasColumnName("position")
                .HasMaxLength(200));

        builder.OwnsOne(j => j.JobUrl, vo =>
            vo.Property(x => x.Value)
                .HasColumnName("job_url")
                .HasMaxLength(2048));

        builder.OwnsOne(j => j.SalaryRange, vo =>
        {
            vo.Property(x => x.Min).HasColumnName("salary_min");
            vo.Property(x => x.Max).HasColumnName("salary_max");
            vo.Property(x => x.Currency).HasColumnName("salary_range").HasMaxLength(3);
        });

        builder.Property(j => j.Status)
            .HasConversion<string>();
    }
}