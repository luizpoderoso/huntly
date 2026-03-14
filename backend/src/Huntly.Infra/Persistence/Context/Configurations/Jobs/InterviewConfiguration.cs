using Huntly.Core.Jobs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Huntly.Infra.Persistence.Context.Configurations.Jobs;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.Property(i => i.Type)
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(i => i.Outcome)
            .HasMaxLength(50)
            .HasConversion<string>();
    }
}