namespace api.DTOs;

// What we send back after a successful register/login. The client stores the
// token and attaches it as "Authorization: Bearer <token>" on later requests.
public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
