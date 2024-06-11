using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Arshdny_ProjectV2.Models;

namespace Arshdny_ProjectV2.AppDbContext
{
    public partial class AppDbContextt : DbContext
    {
        public AppDbContextt()
        {
        }

        public AppDbContextt(DbContextOptions<AppDbContextt> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<AllJob> AllJobs { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Job> Jobs { get; set; } = null!;
        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<Refugee> Refugees { get; set; } = null!;
        public virtual DbSet<RefugeeAppliedJob> RefugeeAppliedJobs { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<HelpingRefugee> HelpingRefugees { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=.;Database=Arshdny;Trusted_Connection=True;TrustServerCertificate=True;");
        //    }
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.;Database=ArshdnyFinal;Trusted_Connection=True;TrustServerCertificate=True;");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.AdminId).HasColumnName("AdminID");

                entity.Property(e => e.Qualification).HasMaxLength(100);

                entity.Property(e => e.Roles).HasMaxLength(100);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                
            });

            modelBuilder.Entity<AllJob>(entity =>
            {
                entity.HasKey(e => e.RefugeeJobId)
                    .HasName("PK_jobss");

                entity.Property(e => e.RefugeeJobId)
                    .ValueGeneratedNever()
                    .HasColumnName("RefugeeJobID");

                entity.Property(e => e.JobTitle).HasMaxLength(100);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryId)
                    .ValueGeneratedNever()
                    .HasColumnName("CountryID");

                entity.Property(e => e.CountryName).HasMaxLength(200);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.Country).HasMaxLength(200);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.JobName).HasMaxLength(100);

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.PublishDate).HasColumnType("date");

                entity.Property(e => e.Salary).HasColumnType("smallmoney");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Phone1).HasMaxLength(20);

                entity.Property(e => e.Phone2).HasMaxLength(20);
            });

            modelBuilder.Entity<Refugee>(entity =>
            {
                entity.Property(e => e.RefugeeId).HasColumnName("RefugeeID");

                entity.Property(e => e.CardEndDate).HasColumnType("date");

                entity.Property(e => e.CardStartDate).HasColumnType("date");

                entity.Property(e => e.CountryId).HasColumnName("CountryID");

                entity.Property(e => e.Cv)
                    .HasMaxLength(500)
                    .HasColumnName("CV");


                entity.Property(e => e.ImagePath).HasMaxLength(500);

                entity.Property(e => e.NationaltyId).HasColumnName("NationaltyID");

                entity.Property(e => e.RefugeeCardNo).HasMaxLength(100);

                entity.Property(e => e.RefugeeJobId).HasColumnName("RefugeeJobID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                
            });

            modelBuilder.Entity<RefugeeAppliedJob>(entity =>
            {
                entity.HasKey(e => new { e.RefugeeId, e.JobId });

                entity.Property(e => e.RefugeeId).HasColumnName("RefugeeID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.ApplyDate).HasColumnType("date");

                entity.Property(e => e.ApplyDate).HasColumnType("JobStatus");


            });

           

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.UserName).HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");



            });


            modelBuilder.Entity<HelpingRefugee>(entity =>
            {
                entity.HasKey(e => new { e.RequestID});

                entity.Property(e => e.RefugeeID).HasColumnName("RefugeeID");

                entity.Property(e => e.RequestDate).HasColumnType("RequestDate");

                entity.Property(e => e.Message).HasColumnType("Message");


            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
