using Microsoft.EntityFrameworkCore;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Infrastructure;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<Challenge> Challenges => Set<Challenge>();
  public DbSet<Membership> Memberships => Set<Membership>();
  public DbSet<ProgressEntry> ProgressEntries => Set<ProgressEntry>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Challenge>()
        .HasOne(c => c.Owner)
        .WithMany(u => u.OwnedChallenges)
        .HasForeignKey(c => c.OwnerId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Membership>()
        .HasOne(m => m.User)
        .WithMany(u => u.Memberships)
        .HasForeignKey(m => m.UserId);

    modelBuilder.Entity<Membership>()
        .HasOne(m => m.Challenge)
        .WithMany(c => c.Memberships)
        .HasForeignKey(m => m.ChallengeId);

    modelBuilder.Entity<ProgressEntry>()
        .HasOne(p => p.User)
        .WithMany(u => u.ProgressEntries)
        .HasForeignKey(p => p.UserId);

    modelBuilder.Entity<ProgressEntry>()
        .HasOne(p => p.Challenge)
        .WithMany(c => c.ProgressEntries)
        .HasForeignKey(p => p.ChallengeId);

    // One progress entry per user per challenge per day
    modelBuilder.Entity<ProgressEntry>()
        .HasIndex(p => new { p.UserId, p.ChallengeId, p.LoggedAt })
        .IsUnique();
  }
}
