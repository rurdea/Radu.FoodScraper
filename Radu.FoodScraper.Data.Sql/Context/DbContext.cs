using Microsoft.EntityFrameworkCore;
using Radu.FoodScraper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Radu.FoodScraper.Data.Context
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public string ConnectionString { get; private set; }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Scraper> Scrapers { get; set; }

        public DbContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Dish>(dish => 
            {
                dish.ToTable("Dish");
                dish.HasKey(d => d.Id);
                dish.Property(d => d.Id).HasColumnName("DishId");
                dish.HasOne(d => d.Scraper).WithMany(s => s.Dishes).HasForeignKey(d => d.ScraperId);
            });

            builder.Entity<Scraper>(scraper =>
            {
                scraper.ToTable("Scraper");
                scraper.HasKey(s => s.Id);
                scraper.Property(s => s.Id).HasColumnName("ScraperId");
                scraper.Property(s => s.Name).HasColumnName("ScraperName");
                scraper.Property(s => s.Description).HasColumnName("ScraperDescription");
                scraper.Property(s => s.Urls).HasColumnName("ScraperUrls");
            });
        }
    }
}
