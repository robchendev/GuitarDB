namespace Backend.DTOs;

public class Change
{
  public object? Old { get; set; }
  public object? New { get; set; }
}

public class GuitarRevisionDiffDto
{
  public int GuitarId { get; set; }
  public Dictionary<string, Change> Changes { get; set; } = new();
}