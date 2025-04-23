namespace Dap.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;  // Added Email
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";  // Default role is User
    }
}
