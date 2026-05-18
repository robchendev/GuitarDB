using Backend.Common.Params;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.Revision;

namespace Backend.Interfaces.Repository;

public interface IGuitarRepository : IRevisableEntityRepository<
  Guitar,
  GuitarDto,
  GuitarSearchParams,
  GuitarExistsParams,
  GuitarRevision,
  GuitarRevisionSnapshotDto,
  GuitarCreateRevisionDto
>
{
}
