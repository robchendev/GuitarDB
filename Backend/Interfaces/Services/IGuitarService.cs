using Backend.Common.Params;
using Backend.DTOs;

namespace Backend.Interfaces.Services;

public interface IGuitarService : IRevisableEntityService<
  GuitarDto,
  GuitarSearchParams,
  GuitarRevisionSnapshotDto,
  GuitarCreateRevisionDto
>
{
}