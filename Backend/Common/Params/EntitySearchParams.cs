using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Params;

public abstract class PaginationParams
{
  private const int MaxPageSize = 50;
  public int Page { get; set; } = 1;
  private int _pageSize = 10; // default pageSize
  public int PageSize
  {
    get => _pageSize;
    set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
  }
}

public class GuitarSearchParams : PaginationParams
{
  [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
  public string? Name { get; set; }
}

public class LuthierSearchParams : PaginationParams
{
  [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
  public string? Name { get; set; }

  // TODO: country search
}
