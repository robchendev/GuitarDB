namespace Backend.Common.Params;

public abstract class EntityExistsParams
{
  public int? ExcludeId { get; set; }
}

public class GuitarExistsParams : EntityExistsParams
{
  public required string Name { get; set; }
  public int? BrandId { get; set; }
  public int? LuthierId { get; set; }
}

public class LuthierExistsParams : EntityExistsParams
{
  public required string Name { get; set; }
  public string? CountryId { get; set; }
}

