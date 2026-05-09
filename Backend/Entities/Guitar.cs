using Backend.Entities.Abstract;
using Backend.Entities.Revision;

namespace Backend.Entities;

public class Guitar : BaseGuitarEntity
{
  public int Id { get; set; }
  public ICollection<GuitarRevision> Revisions { get; set; } = [];
  public ICollection<SoundProfileVote> SoundProfileVotes = [];
}
