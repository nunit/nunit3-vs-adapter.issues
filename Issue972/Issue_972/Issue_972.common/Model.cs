using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Issue_972.common
{ 
    public class AdministrationContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTimeBlock> EmployeeTimeBlocks { get; set; }

        public string DbPath { get; private set; }


        public AdministrationContext(DbContextOptions<AdministrationContext> options): base(options)
        { 
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"SERVER=server Name;Database=Database Name;Trusted_Connection=true");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeTimeBlock>()
                .HasKey(a => new { a.EmployeeId, a.TimeBlockId });

            modelBuilder.Entity<EmployeeTimeBlock>()
                .Property(a => a.Start).HasColumnType("date");

            modelBuilder.Entity<EmployeeTimeBlock>()
                .Property(a => a.End).HasColumnType("date");
        }
    }
}