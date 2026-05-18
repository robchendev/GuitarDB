using Backend.Common.Interfaces;
using Backend.DTOs.Abstract;

namespace Backend.DTOs;

public class LuthierDto : BaseLuthierDto, IHasId
{
  public required int Id { get; set; }
}
