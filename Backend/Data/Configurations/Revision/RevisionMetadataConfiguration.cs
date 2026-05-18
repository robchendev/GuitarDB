using Backend.Entities.Revision;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Revision;

public class RevisionMetadataConfiguration : IEntityTypeConfiguration<RevisionMetadata>
{
  public void Configure(EntityTypeBuilder<RevisionMetadata> builder)
  {
    // RevisionMetadata > RevisionMetadata 
    builder
      .HasOne(rm => rm.ParentRevision)
      .WithMany()
      .HasForeignKey(rm => rm.ParentRevisionId)
      .OnDelete(DeleteBehavior.SetNull);

    builder.HasIndex(rm => rm.Status);
    builder.HasIndex(rm => rm.Version);
    builder.HasIndex(rm => rm.ParentRevisionId);
    builder.HasIndex(rm => rm.CreatedById);

    // concurrency (replacement for rowVersion comparison)
    builder
      .Property(rm => rm.RowVersion)
      .IsRowVersion();
  }
}
