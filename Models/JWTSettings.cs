using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Novels.Models
{
    public class JWTSettings
    {
        public string Secretkey { get; set; }

        public static SecurityToken GenerateToken(string secretekey,User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretekey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,Convert.ToString(user.Id))
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return token;
        }

        public static RefreshTokne GenerateRefreshToken()
        {
            RefreshTokne refresh_token = new RefreshTokne();
            var random_num = new byte[32];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random_num);
                refresh_token.Token = Convert.ToBase64String(random_num);
            }
            refresh_token.ExpiryDate = DateTime.UtcNow.AddMonths(6);
            return refresh_token;
        }

        public static User GetUserFromAccessToken(string access_token, string secret_key, NovelStoreContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret_key);
            var TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(access_token,
                                                       TokenValidationParameters, out securityToken);
            JwtSecurityToken jwttoken = securityToken as JwtSecurityToken;
            if (jwttoken != null && jwttoken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var userId = principle.FindFirst(ClaimTypes.Name)?.Value;
                var user = context.Users.Where(u => u.Id == Convert.ToInt32(userId)).FirstOrDefault();
                context.Entry(user)
                       .Reference(u => u.UserTypeNavigation)
                       .Load();
                return user;


            }
            return null;
        }

        public static bool ValidateRefrestToken(User user,string refresh_token, NovelStoreContext context)
        {
             RefreshTokne token = context.RefreshToknes.Where(r => r.Token == refresh_token)
                                  .OrderByDescending(ob => ob.ExpiryDate).FirstOrDefault();
            if (token != null && token.UserId == user.Id && token.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }
    }
}
