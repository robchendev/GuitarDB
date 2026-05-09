using Backend.Data.Configurations.Property;
using Backend.Entities.Revision;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations.Revision;

public class GuitarRevisionConfiguration : IEntityTypeConfiguration<GuitarRevision>
{
  public void Configure(EntityTypeBuilder<GuitarRevision> builder)
  {
    GuitarPropertyConfiguration.ApplyCommonProperties(builder);

    builder.HasIndex(gr => gr.GuitarId);

    // GuitarRevision > Guitar
    builder
      .HasOne(gr => gr.Guitar)
      .WithMany(g => g.Revisions)
      .HasForeignKey(gr => gr.GuitarId)
      .OnDelete(DeleteBehavior.SetNull);

    // GuitarRevision - RevisionMetadata
    builder
      .HasOne(gr => gr.RevisionMetadata)
      .WithOne(rm => rm.GuitarRevision)
      .HasForeignKey<GuitarRevision>(gr => gr.Id)
      .OnDelete(DeleteBehavior.Cascade); // Delete GuitarRevision if RevisionMetadata is deleted
    // EFcore sees RevisionMetadata as the principal, and GuitarMetadata as the dependent.
    // So if I want to delete GuitarRevision, I should delete the RevisionMetadata instead.
    // Doing so will delete the RevisionMetadata and then it will cascade and delete the GuitarRevision as well.
    // Deleting GuitarRevision is going to orphan the RevisionMetadata.
  }
}
