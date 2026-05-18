using Backend.Common.Interfaces;
using Backend.DTOs.Abstract;

namespace Backend.DTOs;

public class LuthierExerptDto
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
}

public class GuitarDto : BaseGuitarDto, IHasId
{
  public required int Id { get; set; }
}
