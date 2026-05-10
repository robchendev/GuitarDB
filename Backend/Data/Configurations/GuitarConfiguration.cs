using Backend.Data.Configurations.Property;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class RoasterConfiguration : IEntityTypeConfiguration<Guitar>
{
  public void Configure(EntityTypeBuilder<Guitar> builder)
  {
    GuitarPropertyConfiguration.ApplyCommonProperties(builder);

    // Guitar (composite unique on name, brand)
    // TODO: Ensure this is handled in the service/repo layers
    builder
      .HasIndex(r => new { r.Name, r.BrandId })
      .IsUnique();

    // Guitars > Luthier
    builder
      .HasOne(g => g.Luthier)
      .WithMany()
      .HasForeignKey(g => g.LuthierId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}