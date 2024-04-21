using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System;
using TopFarmerWebServer.Data;
using static System.Net.Mime.MediaTypeNames;
using static TopFarmerWebServer.DB.DataModel;

namespace TopFarmerWebServer.DB
{
    public class AppDbContext :DbContext
    {
        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<PlayerDb> Players { get; set; }
        public DbSet<ItemDb> Items { get; set; }

        static readonly ILoggerFactory _logger = LoggerFactory.Create(builder => { builder.AddConsole(); });
        string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GameDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                //.UseLoggerFactory(_logger)
               .UseSqlServer(ConfigManager.Config == null ? _connectionString : ConfigManager.Config.connectionString);

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AccountDb>()
                .HasIndex(a=>a.AccountName)
                .IsUnique();

            builder.Entity<PlayerDb>()
                .HasIndex(a=>a.PlayerName)
                .IsUnique();
        }

    }
}
