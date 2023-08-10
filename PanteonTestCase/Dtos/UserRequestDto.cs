using PanteonTestCase.Context;
using PanteonTestCase.Security;

namespace PanteonTestCase.Dtos
{
    public class UserRequestDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserResponseDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Token Token { get; set; }
    }
}
