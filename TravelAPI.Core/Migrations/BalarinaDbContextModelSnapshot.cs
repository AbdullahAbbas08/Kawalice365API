﻿// <auto-generated />
using System;
using BalarinaAPI.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BalarinaAPI.Core.Migrations
{
    [DbContext(typeof(BalarinaDatabaseContext))]
    partial class BalarinaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("BalarinaAPI.Core.Model.ADPLACEHOLDER", b =>
                {
                    b.Property<int>("ADPlaceholderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("ADPlaceholderCode")
                        .HasColumnType("int");

                    b.Property<int?>("ADStylesADStyleId")
                        .HasColumnType("int");

                    b.Property<int?>("ADTargetsADTargetID")
                        .HasColumnType("int");

                    b.Property<int>("AdStyleID")
                        .HasColumnType("int");

                    b.Property<int>("AdTargetId")
                        .HasColumnType("int");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ADPlaceholderID");

                    b.HasIndex("ADStylesADStyleId");

                    b.HasIndex("ADTargetsADTargetID");

                    b.ToTable("ADPLACEHOLDER");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ADS", b =>
                {
                    b.Property<int>("AdId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("AdTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PlaceHolderID")
                        .HasColumnType("int");

                    b.Property<DateTime>("PublishEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PublishStartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("AdId");

                    b.HasIndex("ClientID");

                    b.HasIndex("PlaceHolderID");

                    b.ToTable("ADS");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ADSTYLES", b =>
                {
                    b.Property<int>("ADStyleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<float>("ADHeight")
                        .HasColumnType("real");

                    b.Property<string>("ADStyleTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("ADWidth")
                        .HasColumnType("real");

                    b.HasKey("ADStyleId");

                    b.ToTable("ADSTYLES");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ADTARGETS", b =>
                {
                    b.Property<int>("ADTargetID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ADTargetTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ADTargetType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ItemID")
                        .HasColumnType("int");

                    b.HasKey("ADTargetID");

                    b.ToTable("ADTARGETS");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CategoryID")
                        .UseIdentityColumn();

                    b.Property<string>("CategoryDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategoryImg")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryOrder")
                        .HasColumnType("int");

                    b.Property<string>("CategoryTitle")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("CategoryViews")
                        .HasColumnType("int");

                    b.Property<bool>("CategoryVisible")
                        .HasColumnType("bit");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("CategoryId");

                    b.ToTable("categories");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Episode", b =>
                {
                    b.Property<int>("EpisodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("EpisodeID")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("DislikeRate")
                        .HasColumnType("int");

                    b.Property<string>("EpisodeDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EpisodeIamgePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EpisodePublishDate")
                        .HasColumnType("datetime");

                    b.Property<string>("EpisodeTitle")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("EpisodeViews")
                        .HasColumnType("int");

                    b.Property<bool>("EpisodeVisible")
                        .HasColumnType("bit");

                    b.Property<int?>("LikeRate")
                        .HasColumnType("int");

                    b.Property<int?>("SessionId")
                        .HasColumnType("int")
                        .HasColumnName("sessionID");

                    b.Property<string>("YoutubeUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EpisodeId");

                    b.HasIndex("SessionId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.EpisodesKeyword", b =>
                {
                    b.Property<int>("EpisodeKeywordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("EpisodeKeywordID")
                        .UseIdentityColumn();

                    b.Property<int?>("EpisodeId")
                        .HasColumnType("int")
                        .HasColumnName("EpisodeID");

                    b.Property<int?>("KeywordId")
                        .HasColumnType("int")
                        .HasColumnName("KeywordID");

                    b.HasKey("EpisodeKeywordId");

                    b.HasIndex(new[] { "EpisodeId" }, "IX_Episodes_Keywords_EpisodeID");

                    b.HasIndex(new[] { "KeywordId" }, "IX_Episodes_Keywords_KeywordID");

                    b.ToTable("Episodes_Keywords");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.EpisodesRelatedForRecentlyModel", b =>
                {
                    b.Property<int>("CategoryId")
                        .HasColumnType("int")
                        .HasColumnName("CategoryID");

                    b.Property<string>("CategoryTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EpisodeDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EpisodeId")
                        .HasColumnType("int")
                        .HasColumnName("EpisodeID");

                    b.Property<string>("EpisodeImg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EpisodePublishDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EpisodeTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EpisodeUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EpisodeViews")
                        .HasColumnType("int");

                    b.Property<bool>("EpisodeVisible")
                        .HasColumnType("bit");

                    b.Property<int>("ProgramId")
                        .HasColumnType("int")
                        .HasColumnName("ProgramID");

                    b.Property<string>("ProgramImg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProgramName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProgramTypeId")
                        .HasColumnType("int")
                        .HasColumnName("ProgramTypeID");

                    b.Property<string>("ProgramTypeTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SeasonTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SessionId")
                        .HasColumnType("int");

                    b.ToTable("EpisodesRelatedForRecentlyModel");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Interviewer", b =>
                {
                    b.Property<int>("InterviewerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("InterviewerID")
                        .UseIdentityColumn();

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("FacebookUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstgramUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InterviewerCover")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InterviewerDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InterviewerName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("InterviewerPicture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LinkedInUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TiktokUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TwitterUrl")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("twitterUrl");

                    b.Property<string>("WebsiteUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("YoutubeUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InterviewerId");

                    b.ToTable("Interviewers");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Keyword", b =>
                {
                    b.Property<int>("KeywordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("KeywordID")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("KeywordTitle")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("KeywordId");

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Program", b =>
                {
                    b.Property<int>("ProgramId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProgramID")
                        .UseIdentityColumn();

                    b.Property<int>("CategoryId")
                        .HasColumnType("int")
                        .HasColumnName("CategoryID");

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("InterviewerId")
                        .HasColumnType("int")
                        .HasColumnName("InterviewerID");

                    b.Property<string>("ProgramDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProgramIDFk")
                        .HasColumnType("int");

                    b.Property<string>("ProgramImg")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProgramName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("ProgramOrder")
                        .HasColumnType("int");

                    b.Property<string>("ProgramPromoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ProgramStartDate")
                        .HasColumnType("datetime");

                    b.Property<int>("ProgramTypeId")
                        .HasColumnType("int")
                        .HasColumnName("ProgramTypeID");

                    b.Property<int>("ProgramViews")
                        .HasColumnType("int");

                    b.Property<bool?>("ProgramVisible")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("((1))")
                        .HasComment("(0) non-Visible - (1) Visible");

                    b.HasKey("ProgramId");

                    b.HasIndex("ProgramIDFk");

                    b.HasIndex(new[] { "CategoryId" }, "IX_Programs_CategoryID");

                    b.HasIndex(new[] { "InterviewerId" }, "IX_Programs_InterviewerID");

                    b.HasIndex(new[] { "ProgramTypeId" }, "IX_Programs_ProgramTypeID");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ProgramType", b =>
                {
                    b.Property<int>("ProgramTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProgramTypeID")
                        .UseIdentityColumn();

                    b.Property<string>("ProgramTypeImgPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProgramTypeOrder")
                        .HasColumnType("int");

                    b.Property<string>("ProgramTypeTitle")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("ProgramTypeViews")
                        .HasColumnType("int");

                    b.HasKey("ProgramTypeId");

                    b.ToTable("ProgramTypes");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Seasons", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SessionID")
                        .UseIdentityColumn();

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProgramId")
                        .HasColumnType("int")
                        .HasColumnName("ProgramID");

                    b.Property<int>("SeasonViews")
                        .HasColumnType("int");

                    b.Property<string>("SessionTitle")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("SessionId");

                    b.HasIndex("ProgramId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Models.Notification", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Descriptions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EpisodeID")
                        .HasColumnType("int");

                    b.Property<string>("IMG")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Visible")
                        .HasColumnType("bit");

                    b.Property<string>("title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("EpisodeID");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Models.Sliders", b =>
                {
                    b.Property<int>("SliderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("ProgramIDFk")
                        .HasColumnType("int");

                    b.Property<string>("SliderImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SliderOrder")
                        .HasColumnType("int");

                    b.Property<string>("SliderTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SliderViews")
                        .HasColumnType("int");

                    b.HasKey("SliderId");

                    b.ToTable("Sliders");
                });

            modelBuilder.Entity("BalarinaAPI.Core.ViewModel.Interviewer.SuperStarModel", b =>
                {
                    b.Property<int>("EpisodeViews")
                        .HasColumnType("int");

                    b.Property<string>("InterviewerDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InterviewerID")
                        .HasColumnType("int");

                    b.Property<string>("InterviewerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InterviewerPicture")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("SuperStarModel");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("TravelAPI.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("TravelAPI.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("LogoPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ADPLACEHOLDER", b =>
                {
                    b.HasOne("BalarinaAPI.Core.Model.ADSTYLES", "ADStyles")
                        .WithMany("ADPLACEHOLDERS")
                        .HasForeignKey("ADStylesADStyleId");

                    b.HasOne("BalarinaAPI.Core.Model.ADTARGETS", "ADTargets")
                        .WithMany("ADPLACEHOLDERS")
                        .HasForeignKey("ADTargetsADTargetID");

                    b.Navigation("ADStyles");

                    b.Navigation("ADTargets");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ADS", b =>
                {
                    b.HasOne("TravelAPI.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ClientID");

                    b.HasOne("BalarinaAPI.Core.Model.ADPLACEHOLDER", "ADPLACEHOLDER")
                        .WithMany()
                        .HasForeignKey("PlaceHolderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ADPLACEHOLDER");

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Episode", b =>
                {
                    b.HasOne("BalarinaAPI.Core.Model.Seasons", "Session")
                        .WithMany("Episodes")
                        .HasForeignKey("SessionId")
                        .HasConstraintName("Episodes_Sessions_FK")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Session");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.EpisodesKeyword", b =>
                {
                    b.HasOne("BalarinaAPI.Core.Model.Episode", "Episode")
                        .WithMany("EpisodesKeywords")
                        .HasForeignKey("EpisodeId")
                        .HasConstraintName("FK_Episodes_Keywords_Episodes")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BalarinaAPI.Core.Model.Keyword", "Keyword")
                        .WithMany("EpisodesKeywords")
                        .HasForeignKey("KeywordId")
                        .HasConstraintName("FK_Episodes_Keywords_Keywords")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Episode");

                    b.Navigation("Keyword");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Program", b =>
                {
                    b.HasOne("BalarinaAPI.Core.Model.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BalarinaAPI.Core.Model.Interviewer", "Interviewer")
                        .WithMany("Programs")
                        .HasForeignKey("InterviewerId")
                        .HasConstraintName("FK_Programs_Interviewers")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BalarinaAPI.Core.Models.Sliders", null)
                        .WithMany("Programs")
                        .HasForeignKey("ProgramIDFk");

                    b.HasOne("BalarinaAPI.Core.Model.ProgramType", "ProgramType")
                        .WithMany("Programs")
                        .HasForeignKey("ProgramTypeId")
                        .HasConstraintName("FK_Programs_ProgramTypes")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Interviewer");

                    b.Navigation("ProgramType");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Seasons", b =>
                {
                    b.HasOne("BalarinaAPI.Core.Model.Program", "Program")
                        .WithMany("Sessions")
                        .HasForeignKey("ProgramId")
                        .HasConstraintName("FK_Sessions_Programs")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Program");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Models.Notification", b =>
                {
                    b.HasOne("BalarinaAPI.Core.Model.Episode", "Episode")
                        .WithMany()
                        .HasForeignKey("EpisodeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Episode");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("TravelAPI.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("TravelAPI.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("TravelAPI.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("TravelAPI.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TravelAPI.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("TravelAPI.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ADSTYLES", b =>
                {
                    b.Navigation("ADPLACEHOLDERS");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ADTARGETS", b =>
                {
                    b.Navigation("ADPLACEHOLDERS");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Episode", b =>
                {
                    b.Navigation("EpisodesKeywords");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Interviewer", b =>
                {
                    b.Navigation("Programs");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Keyword", b =>
                {
                    b.Navigation("EpisodesKeywords");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Program", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.ProgramType", b =>
                {
                    b.Navigation("Programs");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Model.Seasons", b =>
                {
                    b.Navigation("Episodes");
                });

            modelBuilder.Entity("BalarinaAPI.Core.Models.Sliders", b =>
                {
                    b.Navigation("Programs");
                });
#pragma warning restore 612, 618
        }
    }
}
