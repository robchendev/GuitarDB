namespace Backend.DTOs.Abstract;

public class BaseBrandDto
{
  public required string Name { get; set; }
  public string? Description { get; set; }
  public CountryExerptDto? Country { get; set; }
}
