using Backend.Interfaces.Repository;

namespace Backend.Interfaces;

public interface IUnitOfWork
{
  IUserRepository UserRepository { get; }
  IGuitarRepository GuitarRepository { get; }
  IRevisionMetadataRepository RevisionMetadataRepository { get; }

  Task<bool> SaveAllAsync();
  bool HasChanges();
}
