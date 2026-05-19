using Backend.Entities;
using Backend.Entities.Catalog;
using Backend.Entities.Revision;
using Backend.Interfaces.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Backend.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<
  // order matters here
  User,
  Role,
  int,
  IdentityUserClaim<int>,
  UserRole,
  IdentityUserLogin<int>,
  IdentityRoleClaim<int>,
  IdentityUserToken<int>
>(options)
{
  // revisionable entitites
  public DbSet<Guitar> Guitars { get; set; }
  public DbSet<Luthier> Luthiers { get; set; }
  public DbSet<Brand> Brands { get; set; } // Equipment Brands

  // catalog entities (do not have navigation nor revision)
  public DbSet<GuitarBodyShape> GuitarBodyShapes { get; set; }
  public DbSet<GuitarBracingType> GuitarBracingTypes { get; set; }
  public DbSet<GuitarType> GuitarTypes { get; set; }
  public DbSet<Material> Materials { get; set; }
  public DbSet<MaterialConstruction> MaterialConstructions { get; set; }
  public DbSet<MaterialGrade> MaterialGrades { get; set; }
  public DbSet<Country> Countries { get; set; }

  // one off entities (do not have revision)
  public DbSet<SoundProfileVote> SoundProfileVotes { get; set; }

  // revision system
  public DbSet<RevisionMetadata> RevisionMetadatas { get; set; }
  public DbSet<GuitarRevision> GuitarRevisions { get; set; }
  public DbSet<LuthierRevision> LuthierRevisions { get; set; }
  public DbSet<BrandRevision> BrandRevisions { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    /***** Setup Relations on Entities with Audit Data *****/
    ConfigureAuditableEntity(builder.Entity<SoundProfileVote>());
    ConfigureAuditableEntity(builder.Entity<RevisionMetadata>());

    /***** Setup Relations on Entities *****/
    builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
  }

  private static void ConfigureAuditableEntity<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class, IAuditable
  {
    builder.HasOne(ae => ae.CreatedBy)
        .WithMany()
        .HasForeignKey(ae => ae.CreatedById)
        .OnDelete(DeleteBehavior.SetNull);

    builder.HasOne(ae => ae.UpdatedBy)
        .WithMany()
        .HasForeignKey(ae => ae.UpdatedById)
        .OnDelete(DeleteBehavior.SetNull);
  }
}
