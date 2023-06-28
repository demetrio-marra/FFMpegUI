﻿// <auto-generated />
using System;
using FFMpegUI.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FFMpegUI.Mvc.Migrations
{
    [DbContext(typeof(FFMpegDbContext))]
    [Migration("20230628190433_renamed errormessage to StatusMessage in ProcessItem")]
    partial class renamederrormessagetoStatusMessageinProcessItem
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FFMpegUI.Persistence.Entities.FFMpegPersistedProcess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long?>("ConvertedFilesTotalSize")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<long?>("SourceFilesTotalSize")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Processes");
                });

            modelBuilder.Entity("FFMpegUI.Persistence.Entities.FFMpegPersistedProcessItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long?>("ConvertedFileId")
                        .HasColumnType("bigint");

                    b.Property<string>("ConvertedFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("ConvertedFileSize")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProcessId")
                        .HasColumnType("int");

                    b.Property<long?>("SourceFileId")
                        .HasColumnType("bigint");

                    b.Property<string>("SourceFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("SourceFileSize")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("StatusMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Successfull")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ProcessId");

                    b.ToTable("ProcessItems");
                });

            modelBuilder.Entity("FFMpegUI.Persistence.Entities.FFMpegPersistedProcessItem", b =>
                {
                    b.HasOne("FFMpegUI.Persistence.Entities.FFMpegPersistedProcess", "Process")
                        .WithMany("Items")
                        .HasForeignKey("ProcessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Process");
                });

            modelBuilder.Entity("FFMpegUI.Persistence.Entities.FFMpegPersistedProcess", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
