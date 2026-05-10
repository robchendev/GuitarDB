using System.ComponentModel.DataAnnotations;

namespace Backend.Common.Params;

public class LuthierParams : PaginationParams
{
  [MaxLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
  public string? Name { get; set; }

  // TODO: country search
}
