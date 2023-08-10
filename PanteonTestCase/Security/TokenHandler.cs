using Microsoft.IdentityModel.Tokens;
using PanteonTestCase.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PanteonTestCase.Security
{
    public static class TokenHandler
    {
        public static Token CreateToken(UserResponseDto user)
        {
            Token token = new Token();
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim("UserId", user.Id.ToString()));

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("PanteonPanteonPanteonPanteonPanteon"));

            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);



            JwtSecurityToken jwtSecurityToken = new(
                issuer: "localhost",
                audience: "localhost",
                expires: DateTime.Now.AddDays(10),
                notBefore: DateTime.Now,
                signingCredentials: credentials,
                claims: claims
                );

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            token.AccessToken = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            byte[] numbers = new byte[32];
            using RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(numbers);
            token.RefreshToken = Convert.ToBase64String(numbers);
            return token;
        }
    }
}
