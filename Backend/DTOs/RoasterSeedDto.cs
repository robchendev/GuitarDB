namespace Backend.DTOs;

public class GuitarSeedDto
{
  public string Name { get; set; } = string.Empty;
  public string? Alias { get; set; }
  public string? LocationAddress { get; set; }
  public CoordinatesDto? LocationCoordinates { get; set; }
  public string? WebsiteUrl { get; set; }
  public string? Description { get; set; }
}