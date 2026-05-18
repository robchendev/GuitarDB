using System.ComponentModel.DataAnnotations.Schema;
using Backend.Entities.Revision;

namespace Backend.Entities.Abstract;

public interface IRevisionEntity
{
  public int Id { get; } // Created to be the same as RevisionMetadata Id, so this cannot be autoincremented
  public RevisionMetadata RevisionMetadata { get; set; } // uses Id as FK
}
