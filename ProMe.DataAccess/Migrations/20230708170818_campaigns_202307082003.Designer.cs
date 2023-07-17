﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProMe.DataAccess;

#nullable disable

namespace ProMe.DataAccess.Migrations
{
    [DbContext(typeof(ProMeDBContext))]
    [Migration("20230708170818_campaigns_202307082003")]
    partial class campaigns_202307082003
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-preview.4.23259.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CampaignContact", b =>
                {
                    b.Property<Guid>("CampaignsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContactsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CampaignsId", "ContactsId");

                    b.HasIndex("ContactsId");

                    b.ToTable("CampaignContact");
                });

            modelBuilder.Entity("ProMe.DataAccess.Models.Campaign", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ActiveFrom")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("ActiveTo")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("CampaignManagerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("CampaignManagerId");

                    b.ToTable("Campaign");
                });

            modelBuilder.Entity("ProMe.DataAccess.Models.CampaignManager", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccessKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(254)
                        .HasColumnType("nvarchar(254)");

                    b.Property<DateTimeOffset>("LastKeyReset")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AccessKey")
                        .IsUnique();

                    b.ToTable("CampaignManagers");
                });

            modelBuilder.Entity("ProMe.DataAccess.Models.Contact", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasMaxLength(254)
                        .HasColumnType("nvarchar(254)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(350)
                        .HasColumnType("nvarchar(350)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("ProMe.DataAccess.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(254)
                        .HasColumnType("nvarchar(254)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("LastPasswordReset")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Verified")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CampaignContact", b =>
                {
                    b.HasOne("ProMe.DataAccess.Models.Campaign", null)
                        .WithMany()
                        .HasForeignKey("CampaignsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProMe.DataAccess.Models.Contact", null)
                        .WithMany()
                        .HasForeignKey("ContactsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProMe.DataAccess.Models.Campaign", b =>
                {
                    b.HasOne("ProMe.DataAccess.Models.CampaignManager", "CampaignManager")
                        .WithMany("Campaigns")
                        .HasForeignKey("CampaignManagerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CampaignManager");
                });

            modelBuilder.Entity("ProMe.DataAccess.Models.Contact", b =>
                {
                    b.HasOne("ProMe.DataAccess.Models.User", "User")
                        .WithMany("Contacts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProMe.DataAccess.Models.CampaignManager", b =>
                {
                    b.Navigation("Campaigns");
                });

            modelBuilder.Entity("ProMe.DataAccess.Models.User", b =>
                {
                    b.Navigation("Contacts");
                });
#pragma warning restore 612, 618
        }
    }
}
