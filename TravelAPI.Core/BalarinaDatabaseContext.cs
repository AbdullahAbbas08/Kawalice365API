using System;
using BalarinaAPI.Core.Models;
using BalarinaAPI.Core.ViewModel.Interviewer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TravelAPI.Models;

#nullable disable

namespace BalarinaAPI.Core.Model
{
    public partial class BalarinaDatabaseContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public BalarinaDatabaseContext(DbContextOptions<BalarinaDatabaseContext> options)
            : base(options)
        {
            base.ChangeTracker.AutoDetectChangesEnabled = false;
        }


        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Episode> Episodes { get; set; }
        public virtual DbSet<EpisodesKeyword> EpisodesKeywords { get; set; }
        public virtual DbSet<EpisodesRelatedForRecentlyModel> EpisodesRelatedForRecentlyModels { get; set; }
        public virtual DbSet<Interviewer> Interviewers { get; set; }
        public virtual DbSet<Keyword> Keywords { get; set; }
        public virtual DbSet<Program> Programs { get; set; }
        public virtual DbSet<ProgramType> ProgramTypes { get; set; }
        public virtual DbSet<Seasons> Sessions { get; set; }
        public virtual DbSet<Sliders> Sliders { get; set; } 
        public virtual DbSet<SuperStarModel> SuperStarModel { get; set; } 
        public virtual DbSet<ADTARGETS> ADTARGETS { get; set; } 
        public virtual DbSet<ADSTYLES> ADSTYLES { get; set; } 
        public virtual DbSet<ADPLACEHOLDER> ADPLACEHOLDER { get; set; } 
        public virtual DbSet<ADS> ADS  { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-QK89E21\\SQLEXPRESS;Initial Catalog=BalarinaDatabase;User ID=travesta;Password=travesta123456");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<AspNetRole>(entity =>
            //{
            //    entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
            //        .IsUnique()
            //        .HasFilter("([NormalizedName] IS NOT NULL)");

            //    entity.Property(e => e.Name).HasMaxLength(256);

            //    entity.Property(e => e.NormalizedName).HasMaxLength(256);
            //});

            //modelBuilder.Entity<AspNetRoleClaim>(entity =>
            //{
            //    entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            //    entity.Property(e => e.RoleId).IsRequired();

            //    entity.HasOne(d => d.Role)
            //        .WithMany(p => p.AspNetRoleClaims)
            //        .HasForeignKey(d => d.RoleId);
            //});

            //modelBuilder.Entity<AspNetUser>(entity =>
            //{
            //    entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            //    entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
            //        .IsUnique()
            //        .HasFilter("([NormalizedUserName] IS NOT NULL)");

            //    entity.Property(e => e.Email).HasMaxLength(256);

            //    entity.Property(e => e.FirstName)
            //        .IsRequired()
            //        .HasMaxLength(100);

            //    entity.Property(e => e.LastName)
            //        .IsRequired()
            //        .HasMaxLength(255);

            //    entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

            //    entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

            //    entity.Property(e => e.UserName).HasMaxLength(256);
            //});

            //modelBuilder.Entity<AspNetUserClaim>(entity =>
            //{
            //    entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            //    entity.Property(e => e.UserId).IsRequired();

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.AspNetUserClaims)
            //        .HasForeignKey(d => d.UserId);
            //});

            //modelBuilder.Entity<AspNetUserLogin>(entity =>
            //{
            //    entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            //    entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            //    entity.Property(e => e.UserId).IsRequired();

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.AspNetUserLogins)
            //        .HasForeignKey(d => d.UserId);
            //});

            //modelBuilder.Entity<AspNetUserRole>(entity =>
            //{
            //    entity.HasKey(e => new { e.UserId, e.RoleId });

            //    entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

            //    entity.HasOne(d => d.Role)
            //        .WithMany(p => p.AspNetUserRoles)
            //        .HasForeignKey(d => d.RoleId);

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.AspNetUserRoles)
            //        .HasForeignKey(d => d.UserId);
            //});

            //modelBuilder.Entity<AspNetUserToken>(entity =>
            //{
            //    entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            //    entity.HasOne(d => d.User)
            //        .WithMany(p => p.AspNetUserTokens)
            //        .HasForeignKey(d => d.UserId);
            //});

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryDescription).IsRequired();

                entity.Property(e => e.CategoryImg).IsRequired();

                entity.Property(e => e.CategoryTitle)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Episode>(entity =>
            {
                entity.Property(e => e.EpisodeId).HasColumnName("EpisodeID");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EpisodeDescription).IsRequired();

                entity.Property(e => e.EpisodePublishDate).HasColumnType("datetime");

                entity.Property(e => e.EpisodeTitle)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SessionId).HasColumnName("sessionID");

                entity.Property(e => e.YoutubeUrl).IsRequired();

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Episodes)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Episodes_Sessions_FK");
            });

            modelBuilder.Entity<EpisodesKeyword>(entity =>
            {
                entity.HasKey(e => e.EpisodeKeywordId);

                entity.ToTable("Episodes_Keywords");

                entity.HasIndex(e => e.EpisodeId, "IX_Episodes_Keywords_EpisodeID");

                entity.HasIndex(e => e.KeywordId, "IX_Episodes_Keywords_KeywordID");

                entity.Property(e => e.EpisodeKeywordId).HasColumnName("EpisodeKeywordID");

                entity.Property(e => e.EpisodeId).HasColumnName("EpisodeID");

                entity.Property(e => e.KeywordId).HasColumnName("KeywordID");

                entity.HasOne(d => d.Episode)
                    .WithMany(p => p.EpisodesKeywords)
                    .HasForeignKey(d => d.EpisodeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Episodes_Keywords_Episodes");

                entity.HasOne(d => d.Keyword)
                    .WithMany(p => p.EpisodesKeywords)
                    .HasForeignKey(d => d.KeywordId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Episodes_Keywords_Keywords");
            });

            modelBuilder.Entity<EpisodesRelatedForRecentlyModel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EpisodesRelatedForRecentlyModel");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.EpisodeId).HasColumnName("EpisodeID");

                entity.Property(e => e.ProgramId).HasColumnName("ProgramID");

                entity.Property(e => e.ProgramTypeId).HasColumnName("ProgramTypeID");
            });

            modelBuilder.Entity<Interviewer>(entity =>
            {
                entity.Property(e => e.InterviewerId).HasColumnName("InterviewerID");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InterviewerDescription).IsRequired();

                entity.Property(e => e.InterviewerName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.InterviewerPicture).IsRequired();

                entity.Property(e => e.TwitterUrl).HasColumnName("twitterUrl");
            });

            modelBuilder.Entity<Keyword>(entity =>
            {
                entity.Property(e => e.KeywordId).HasColumnName("KeywordID");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.KeywordTitle)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.HasIndex(e => e.CategoryId, "IX_Programs_CategoryID");

                entity.HasIndex(e => e.InterviewerId, "IX_Programs_InterviewerID");

                entity.HasIndex(e => e.ProgramTypeId, "IX_Programs_ProgramTypeID");
               
                entity.Property(e => e.ProgramId).HasColumnName("ProgramID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InterviewerId).HasColumnName("InterviewerID");

                entity.Property(e => e.ProgramDescription).IsRequired();

                entity.Property(e => e.ProgramImg).IsRequired();

                entity.Property(e => e.ProgramName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ProgramStartDate).HasColumnType("datetime");

                entity.Property(e => e.ProgramTypeId).HasColumnName("ProgramTypeID");

                entity.Property(e => e.ProgramVisible)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("(0) non-Visible - (1) Visible");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Programs)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Programs_Categories");

                entity.HasOne(d => d.Interviewer)
                    .WithMany(p => p.Programs)
                    .HasForeignKey(d => d.InterviewerId)
                    .HasConstraintName("FK_Programs_Interviewers");

                entity.HasOne(d => d.ProgramType)
                    .WithMany(p => p.Programs)
                    .HasForeignKey(d => d.ProgramTypeId)
                    .HasConstraintName("FK_Programs_ProgramTypes");
            });

            modelBuilder.Entity<ProgramType>(entity =>
            {
                entity.Property(e => e.ProgramTypeId).HasColumnName("ProgramTypeID");

                entity.Property(e => e.ProgramTypeTitle)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Seasons>(entity =>
            {
                entity.Property(e => e.SessionId).HasColumnName("SessionID");
                entity.HasKey(e => e.SessionId);
                entity.Property(e => e.ProgramId).HasColumnName("ProgramID");

                entity.Property(e => e.SessionTitle).HasMaxLength(255);

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.ProgramId)
                    .HasConstraintName("FK_Sessions_Programs");
            });

            //OnModelCreatingPartial(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
