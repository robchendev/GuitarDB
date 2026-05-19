using Backend.Common.Interfaces;
using Backend.DTOs.Abstract;

namespace Backend.DTOs;

public class BrandDto : BaseBrandDto, IHasId
{
  public required int Id { get; set; }
}
