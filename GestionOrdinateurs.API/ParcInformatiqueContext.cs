using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public class ParcInformatiqueContext : DbContext
{
    public DbSet<Ordinateur> Ordinateurs { get; set; }
    public DbSet<Reparation> Reparations { get; set; }
    public DbSet<User> Users { get; set; }

    public ParcInformatiqueContext()
    {
    }

    public ParcInformatiqueContext(
        DbContextOptions<ParcInformatiqueContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=parcinfo.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Login)
            .IsUnique();

        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(ordinateur => ordinateur.Caracteristiques);

        modelBuilder.Entity<Ordinateur>()
            .HasMany(ordinateur => ordinateur.Reparations)
            .WithOne(reparation => reparation.UnOrdinateur)
            .HasForeignKey(reparation => reparation.OrdinateurId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reparation>()
            .HasOne(reparation => reparation.UnOrdinateur)
            .WithMany(ordinateur => ordinateur.Reparations)
            .HasForeignKey(reparation => reparation.OrdinateurId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}