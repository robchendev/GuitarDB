namespace Backend.DTOs;

public class Change
{
  public object? Old { get; set; }
  public object? New { get; set; }
}

public class RevisionDiffDto // could this be refactored into a single DTO?
{
  public int EntityId { get; set; } // should this be optional? works fine without?
  public Dictionary<string, Change> Changes { get; set; } = new();
  public int Iteration { get; set; }
}