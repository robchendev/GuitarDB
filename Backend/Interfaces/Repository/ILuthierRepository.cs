using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.Revision;

namespace Backend.Interfaces.Repository;

public interface ILuthierRepository : IRevisableEntityRepository<
  Luthier,
  LuthierDto,
  LuthierSearchParams,
  LuthierExistsParams,
  LuthierRevision,
  LuthierRevisionSnapshotDto,
  LuthierCreateRevisionDto
>
{
}