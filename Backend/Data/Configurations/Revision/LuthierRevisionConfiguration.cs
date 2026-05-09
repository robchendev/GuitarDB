using Backend.Entities;
using Backend.Entities.Revision;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Revision;

public class LuthierRevisionConfiguration : IEntityTypeConfiguration<LuthierRevision>
{
  public void Configure(EntityTypeBuilder<LuthierRevision> builder)
  {
    builder.HasIndex(lr => lr.LuthierId);

    // LuthierRevision > Luthier
    builder
      .HasOne(lr => lr.Luthier)
      .WithMany(l => l.Revisions)
      .HasForeignKey(lr => lr.LuthierId)
      .OnDelete(DeleteBehavior.SetNull);

    // LuthierRevision > RevisionMetadata
    builder
      .HasOne(lr => lr.RevisionMetadata)
      .WithOne(rm => rm.LuthierRevision)
      .HasForeignKey<LuthierRevision>(lr => lr.Id)
      .OnDelete(DeleteBehavior.Cascade); // Delete LuthierRevision if RevisionMetadata is deleted
    // EFcore sees RevisionMetadata as the principal, and LuthierMetadata as the dependent.
    // So if I want to delete LuthierRevision, I should delete the RevisionMetadata instead.
    // Doing so will delete the RevisionMetadata and then it will cascade and delete the LuthierRevision as well.
    // Deleting LuthierRevision is going to orphan the RevisionMetadata.
  }
}
