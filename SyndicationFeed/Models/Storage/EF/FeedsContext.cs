﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Models.Storage.EF
{
    public class FeedsContext : IdentityDbContext
    {
        public FeedsContext(DbContextOptions<FeedsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // do not store publications and failure message in the DB,
            // only in cache
            // we are using fluent api in order to avoid polluting
            // common classes with EF-specific attributes
            modelBuilder.Entity<FeedWithDownloadTime>()
                .ToTable("Feeds")
                .Ignore(feed => feed.Publications)
                .Ignore(feed => feed.LoadFailureMessage)
                .Ignore(feed => feed.SourceAddress)
                .Ignore(feed => feed.LastDownloadTime)
                .Ignore(feed => feed.ValidTill)
                .Property(feed => feed.SourceAddressString).IsRequired();

            modelBuilder.Entity<CollectionWithFeeds>()
                .HasMany(coll => coll.Feeds)
                .WithOne()
                .IsRequired();

            modelBuilder.Entity<CollectionWithFeeds>()
                .Property(coll => coll.Name)
                .IsRequired();

            modelBuilder.Entity<CollectionWithFeeds>()
                .HasOne(coll => coll.User)
                .WithMany()
                .IsRequired();
        }
        
        public DbSet<CollectionWithFeeds> Collections { get; set; }
    }
}
