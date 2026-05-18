using Backend.Common.Interfaces;
using Backend.Entities.Abstract;
using Backend.Entities.Revision;

namespace Backend.Entities;

public class Luthier : BaseLuthierEntity, IHasId
{
  public int Id { get; set; }
  public ICollection<LuthierRevision> Revisions { get; set; } = [];
  public ICollection<GuitarRevision> GuitarRevisions = [];
  public ICollection<Guitar> Guitars = [];
}
