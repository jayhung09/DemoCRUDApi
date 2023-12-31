﻿using Microsoft.EntityFrameworkCore;

namespace DemoCRUDApi.Model
{
    public class DemoDBContext : DbContext
    {
        public DemoDBContext()
        {
        }

        public DemoDBContext(DbContextOptions<DemoDBContext> options) : base(options)
        {
        }

        public virtual DbSet<CrudDemo> CrudDemo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DemoCRUDConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrudDemo>().Property(c => c.Id).ValueGeneratedNever();
        }

        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}
