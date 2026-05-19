namespace Backend.Entities.Catalog;

public class Country
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public ICollection<Guitar> Guitars { get; set; } = [];
}
