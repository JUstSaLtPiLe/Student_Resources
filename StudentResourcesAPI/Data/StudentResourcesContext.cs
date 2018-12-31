using Microsoft.EntityFrameworkCore;
using StudentResourcesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Data
{
    public class StudentResourcesContext : DbContext
    {
        public StudentResourcesContext(DbContextOptions<StudentResourcesContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Grade>()
                .HasKey(g => new { g.AccountId, g.SubjectId });

            modelBuilder.Entity<ClazzSubject>()
                .HasKey(cs => new { cs.ClazzId, cs.SubjectId });

            modelBuilder.Entity<StudentClazz>()
                .HasKey(sc => new { sc.AccountId, sc.ClazzId });

            modelBuilder.Entity<RoleAccount>()
                .HasKey(ra => new { ra.RoleId, ra.AccountId });

        }

        public DbSet<Account> Account { get; set; }
        public DbSet<Clazz> Clazz { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<StudentClazz> StudentClazz { get; set; }
        public DbSet<ClazzSubject> ClazzSubject { get; set; }
        public DbSet<Grade> Grade { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RoleAccount> RoleAccount { get; set; }
        public DbSet<GeneralInformation> GeneralInformation { get; set; }
    }

}
