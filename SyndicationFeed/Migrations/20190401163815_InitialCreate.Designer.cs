﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SyndicationFeed.Models.Storage.EF;

namespace SyndicationFeed.Server.Migrations
{
    [DbContext(typeof(FeedsContext))]
    [Migration("20190401163815_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SyndicationFeed.Models.CollectionWithFeeds", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("SyndicationFeed.Models.FeedWithDownloadTime", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CollectionWithFeedsId");

                    b.Property<DateTime>("LastDownloadTime");

                    b.Property<string>("SourceAddressString");

                    b.Property<int>("Type");

                    b.Property<DateTime>("ValidTill");

                    b.HasKey("Id");

                    b.HasIndex("CollectionWithFeedsId");

                    b.ToTable("FeedWithDownloadTime");
                });

            modelBuilder.Entity("SyndicationFeed.Models.FeedWithDownloadTime", b =>
                {
                    b.HasOne("SyndicationFeed.Models.CollectionWithFeeds")
                        .WithMany("Feeds")
                        .HasForeignKey("CollectionWithFeedsId");
                });
#pragma warning restore 612, 618
        }
    }
}