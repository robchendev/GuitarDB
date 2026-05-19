using Backend.Interfaces;
using Backend.Interfaces.Repository;

namespace Backend.Data;

public class UnitOfWork(
  DataContext context,
  IUserRepository userRepository,
  IGuitarRepository guitarRepository,
  ILuthierRepository luthierRepository,
  IBrandRepository brandRepository,
  IRevisionMetadataRepository revisionMetadataRepository
) : IUnitOfWork
{
  public IUserRepository UserRepository => userRepository;
  public IGuitarRepository GuitarRepository => guitarRepository;
  public ILuthierRepository LuthierRepository => luthierRepository;
  public IBrandRepository BrandRepository => brandRepository;
  public IRevisionMetadataRepository RevisionMetadataRepository => revisionMetadataRepository;

  public async Task<bool> SaveAllAsync()
  {
    try
    {
      return await context.SaveChangesAsync() > 0;
    }
    catch
    {
      return false;
    }
  }

  public bool HasChanges()
  {
    return context.ChangeTracker.HasChanges();
  }
}
