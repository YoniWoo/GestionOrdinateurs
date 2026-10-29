using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API.Data;

public class ParcInformatiqueContext : DbContext
{
    public DbSet<Ordinateur> Ordinateurs { get; set; } = null!;

    public DbSet<Reparation> Reparations { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public ParcInformatiqueContext(DbContextOptions<ParcInformatiqueContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=parcinfo.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ordinateur>()
            .HasKey(ordinateur => ordinateur.Id);

        modelBuilder.Entity<Ordinateur>()
            .OwnsOne(ordinateur => ordinateur.Caracteristiques);

        modelBuilder.Entity<Reparation>()
            .HasKey(reparation => reparation.Id);

        modelBuilder.Entity<Ordinateur>()
            .HasMany(ordinateur => ordinateur.Reparations)
            .WithOne(reparation => reparation.UnOrdinateur)
            .HasForeignKey(reparation => reparation.OrdinateurId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasKey(utilisateur => utilisateur.Id);

        modelBuilder.Entity<User>()
            .HasIndex(utilisateur => utilisateur.Login)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(utilisateur => utilisateur.Login)
            .IsRequired();
    }
}