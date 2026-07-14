using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

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

    [Required]
    [StringLength(12, MinimumLength = 12)]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "Friend code must be 12 digits.")]
    public string FriendCode { get; set; } = string.Empty;

}
