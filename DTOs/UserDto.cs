namespace Youtube_Entertainment_Project.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public bool IsVerified { get; set; }
    }
}
