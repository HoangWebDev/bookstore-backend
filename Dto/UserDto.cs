namespace BookStore.Dto
{
    public class UserDto

    {
            public int UserId { get; set; }
            public string? UserName { get; set; }
            public string? PasswordHash { get; set; }
            public string? Role { get; set; }
            public string? FullName { get; set; }
    }
}
