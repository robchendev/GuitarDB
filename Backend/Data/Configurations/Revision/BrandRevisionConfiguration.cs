using Backend.Entities.Revision;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Revision;

public class BrandRevisionConfiguration : IEntityTypeConfiguration<BrandRevision>
{
  public void Configure(EntityTypeBuilder<BrandRevision> builder)
  {
    builder.HasIndex(br => br.BrandId);

    // BrandRevision > Brand
    builder
      .HasOne(br => br.Brand)
      .WithMany(b => b.Revisions)
      .HasForeignKey(br => br.BrandId)
      .OnDelete(DeleteBehavior.SetNull);

    // BrandRevision > RevisionMetadata
    builder
      .HasOne(br => br.RevisionMetadata)
      .WithOne(rm => rm.BrandRevision)
      .HasForeignKey<BrandRevision>(br => br.Id)
      .OnDelete(DeleteBehavior.Cascade); // Delete BrandRevision if RevisionMetadata is deleted
    // EFcore sees RevisionMetadata as the principal, and BrandMetadata as the dependent.
    // So if I want to delete BrandRevision, I should delete the RevisionMetadata instead.
    // Doing so will delete the RevisionMetadata and then it will cascade and delete the BrandRevision as well.
    // Deleting BrandRevision is going to orphan the RevisionMetadata.
  }
}
