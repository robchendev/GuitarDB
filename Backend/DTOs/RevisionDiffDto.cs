namespace Backend.DTOs;

public class Change
{
  public object? Old { get; set; }
  public object? New { get; set; }
}

public class RevisionDiffDto
{
  public int EntityId { get; set; }
  public Dictionary<string, Change> Changes { get; set; } = new();
  public uint RowVersion { get; set; }
}