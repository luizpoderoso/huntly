using System.Reflection;
using Huntly.Core.Auth.Entities;
using Huntly.Core.Job.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Huntly.Infra.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) :  IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}