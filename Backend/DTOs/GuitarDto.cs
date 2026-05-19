using Backend.Common.Interfaces;
using Backend.DTOs.Abstract;

namespace Backend.DTOs;

public class GuitarDto : BaseGuitarDto, IHasId
{
  public required int Id { get; set; }
}
