using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface ILuthierService : IRevisableEntityService<
  LuthierDto,
  LuthierSearchParams,
  LuthierRevisionSnapshotDto,
  LuthierCreateRevisionDto
>
{
}