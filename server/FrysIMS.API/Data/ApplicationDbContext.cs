using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FrysIMS.API.Models;

namespace FrysIMS.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Entity Sets
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMaterial> ProjectMaterials { get; set; }
        public DbSet<MaterialLocation> MaterialLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Optional: Fluent API configurations

            builder.Entity<Project>()
                .HasMany(p => p.ProjectMaterials)
                .WithOne(pm => pm.Project)
                .HasForeignKey(pm => pm.ProjectId);

            builder.Entity<ProjectMaterial>()
                .HasOne(pm => pm.Stock)
                .WithMany()
                .HasForeignKey(pm => pm.StockId);

            builder.Entity<ProjectMaterial>()
                .HasMany(pm => pm.MaterialLocations)
                .WithOne(ml => ml.ProjectMaterial)
                .HasForeignKey(ml => ml.ProjectMaterialId);

            builder.Entity<Stock>()
                .HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId);

            builder.Entity<Project>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId);

            builder.Entity<MaterialLocation>()
                .HasOne(ml => ml.UpdatedByUser)
                .WithMany()
                .HasForeignKey(ml => ml.UpdatedByUserId);
        }
    }
}
