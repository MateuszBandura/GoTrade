using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

// DTOs are the shapes we accept/return over the wire. We never expose the
// AppUser entity directly — it carries the password hash and EF baggage.
public class RegisterDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
