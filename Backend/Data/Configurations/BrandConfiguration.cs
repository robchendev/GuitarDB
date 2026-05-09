using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
  public void Configure(EntityTypeBuilder<Brand> builder)
  {
    builder.Property(b => b.Name).HasMaxLength(100);
    builder.Property(b => b.Description).HasMaxLength(2000);
  }
}
