namespace RacedayMvc.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        // Flattened properties (no nested UserDto)
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}