namespace Backend.DTOs.Abstract;

public class BaseGuitarDto
{
  public required string Name { get; set; }
  public string? Description { get; set; }
  public LuthierExerptDto? Luthier { get; set; }
}
