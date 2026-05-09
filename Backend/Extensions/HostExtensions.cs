using Backend.Data;
using Backend.Data.Seed;
using Backend.Entities;
using Backend.Entities.Catalog;
using Backend.Entities.Revision;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Extensions;

public static class HostExtensions
{
  public static async Task ApplyMigrationsAndSeedDatabase(this IHost host)
  {
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
      var context = services.GetRequiredService<DataContext>();
      var userManager = services.GetRequiredService<UserManager<User>>();
      var roleManager = services.GetRequiredService<RoleManager<Role>>();
      await context.Database.MigrateAsync(); // apply pending migration to DB, create DB if it doesnt exist

      // seed mock data into tables
      await Seed.SeedTable<RevisionMetadata>(context, "RevisionMetadatas.json");
      await Seed.SeedUsers(userManager, roleManager);
      // I think these are only needed instead of SeedTable because they had special geospatial data handling logic
      // await Seed.SeedGuitars(context, "Guitars.json");
      // await Seed.SeedGuitarRevisions(context, "GuitarRevisions.json");
      await Seed.SeedTable<Country>(context, "Countries.json");
      await Seed.SeedTable<Brand>(context, "Brands.json");
      await Seed.SeedTable<Guitar>(context, "Guitars.json");
      await Seed.SeedTable<GuitarRevision>(context, "GuitarRevisions.json");
      await Seed.SeedTable<Luthier>(context, "Luthiers.json");
      await Seed.SeedTable<LuthierRevision>(context, "LuthierRevisions.json");
    }
    catch (Exception ex)
    {
      var logger = services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occurred during migration");
    }
  }
}
