using Backend.Entities.Abstract;

namespace Backend.Entities;

public class SoundProfileVote : BaseAuditableEntity
{
  public int GuitarId { get; set; }
  public Guitar Guitar { get; set; } = null!;

  // sliders from -10 to 10, Bright = -10, Neutral = 0, Warm = 10, displays as a two sided barchart?
  // but for the sake of easier DB, normalize to 0 to 20 once converting to DB
  // add bassiness maybe? whatever is used to describe that
  public byte ToneBrightWarm { get; set; }
  public byte SustainShortLong { get; set; }
  public byte AttackSlowFast { get; set; }
  public byte ClarityMuddyClear { get; set; } // Note Separation
  public byte DynamicRangeSmallLarge { get; set; }
  public byte BodyThinFull { get; set; }
  public byte VolumeQuietLoud { get; set; }
}
