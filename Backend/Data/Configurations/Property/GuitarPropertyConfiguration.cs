using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Property;

public static class GuitarPropertyConfiguration
{
  public static void ApplyCommonProperties<T>(EntityTypeBuilder<T> builder) where T : class
  {
    builder.Property("Name").HasMaxLength(100);
    builder.Property("Description").HasMaxLength(2000);
  }
}