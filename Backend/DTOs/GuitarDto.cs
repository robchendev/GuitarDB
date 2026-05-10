namespace Backend.DTOs;

public class LuthierExerptDto
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
}

public class GuitarDto
{
  public required int Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public LuthierExerptDto? Luthier { get; set; }
}
