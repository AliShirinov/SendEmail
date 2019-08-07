using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SendEmailApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SendEmailApp.Data
{
    public class EmailDbContext : IdentityDbContext<AppUser>
    {
        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options) { }
        public DbSet<Models.UserTask> Tasks { get; set; }
        public DbSet<UserTaskRelation> Shares { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<UserTaskRelation>().HasKey(x => new { x.AppUserId, x.UserTaskId });

            builder.Entity<UserTaskRelation>()
            .HasOne(x => x.AppUser)
            .WithMany(y => y.UserTaskRelations)
            .HasForeignKey(z => z.AppUserId);

            builder.Entity<UserTaskRelation>()
                .HasOne(x => x.UserTask)
                .WithMany(y => y.UserTaskRelations)
                .HasForeignKey(z => z.UserTaskId);

            builder.Entity<IdentityUserLogin<string>>().HasKey(x => x.UserId);
            builder.Entity<IdentityUserRole<string>>().HasKey(x => x.UserId);
            builder.Entity<IdentityUserClaim<string>>().HasKey(x => x.UserId);
            builder.Entity<IdentityUserToken<string>>().HasKey(x => x.UserId);
            builder.Entity<IdentityUser<string>>().HasKey(x => x.Id);

        }
    }
}
