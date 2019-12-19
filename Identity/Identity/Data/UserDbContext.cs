using Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Data
{
    public class UserDbContext : IdentityDbContext<User>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Organization> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(user => user.HasIndex(x => x.Locale)
                                             .IsUnique(false));

            builder.Entity<Organization>(org =>
                {
                    org.ToTable("Organizations");
                    org.HasKey(i => i.Id);

                    org.HasMany<User>().WithOne()
                    .HasForeignKey(i => i.OrganizationId)
                    .IsRequired(false);
            });
        }
    }
}
