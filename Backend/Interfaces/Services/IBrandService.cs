using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IBrandService : IRevisableEntityService<
  BrandDto,
  BrandSearchParams,
  BrandRevisionSnapshotDto,
  BrandCreateRevisionDto
>
{
}