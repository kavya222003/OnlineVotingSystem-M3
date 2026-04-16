using Microsoft.EntityFrameworkCore;
using OnlineVoting_API.Models;

namespace OnlineVoting_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Poll> Polls => Set<Poll>();
        public DbSet<PollOption> PollOptions => Set<PollOption>();
        public DbSet<Vote> Votes => Set<Vote>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<PollTemplate> PollTemplates => Set<PollTemplate>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vote>(e =>
            {
                e.HasIndex(v => new { v.UserId, v.PollId }).IsUnique();

                e.HasOne(v => v.User)
                    .WithMany(u => u.Votes)
                    .HasForeignKey(v => v.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(v => v.Poll)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(v => v.PollId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(v => v.Option)
                    .WithMany(o => o.Votes)
                    .HasForeignKey(v => v.OptionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}