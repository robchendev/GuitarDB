namespace Backend.Common.Interfaces;

/// <summary>
/// Implementors must mark Id as <c>required</c> if used in a DTO.
/// </summary>
public interface IHasId
{
  public int Id { get; set; }
}