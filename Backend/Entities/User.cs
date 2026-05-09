using Microsoft.AspNetCore.Identity;

namespace Backend.Entities;

public class User : IdentityUser<int>
{
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UsernameUpdatedAt { get; set; }

  public int? UpdatedById { get; set; }

  public User? UpdatedBy { get; set; }
  public ICollection<UserRole> UserRoles { get; set; } = [];
}
