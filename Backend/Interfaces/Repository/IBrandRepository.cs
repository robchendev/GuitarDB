using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.Revision;

namespace Backend.Interfaces.Repository;

public interface IBrandRepository : IRevisableEntityRepository<
  int,
  Brand,
  BrandDto,
  BrandSearchParams,
  BrandExistsParams,
  BrandRevision,
  BrandRevisionSnapshotDto,
  BrandCreateRevisionDto
>
{
}
