using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.JwtServices
{
    public class TokenService
    {
        public static string CreateToken(UserLoginTable user)
        {
            var key = Encoding.UTF8.GetBytes(Settings.Secret);
            // 位於 /JwtServices目錄下的 Settings類別 （搭配 System.Text命名空間）

            var descriptor = new SecurityTokenDescriptor
            {
                // 如果您沒有學過之前的「 .NET Core會員登入 (ClaimsIdentity)」課程，這裡就卡住了。
                // 在token裡面搭配 Claims來存放「帳號」、「角色、群組」等個人資料。
                Subject = new ClaimsIdentity(new Claim[]     {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role,    user.UserRank.ToString())   // 將來可用於存放  這名使用者的角色、群組 (Role)

                    // 如果您要用 JwtRegisteredClaim，請看 https://blog.poychang.net/authenticating-jwt-tokens-in-asp-net-core-webapi/
                    //new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub,  user.UserName),
                    //new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti,     user.UserId.ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(1.0),   // 使用期限，多久會過期？
                // token 一個小時就過期。也可寫成 DateTime.UtcNow.AddHours(EXPIRE_HOURS)
                // token不像Session可以強制註銷、清除（Session.Abandon()）。只能等待它過期失效。

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                // 簽章。  透過  Hash雜湊運算（單向打亂，無法復原）
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);  // 最後，傳回 token字串。內容如下（以英文句號（.）區隔，分成三個段落）：
            // "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
            //                  eyJ1bmlxdWVfbmFtZSI6Im1pczIwMDBsYWIiLCJyb2xlIjoiMTAwIiwibmJmIjoxNjIyOTA2NDk0LCJleHAiOjE2MjI5MTAwOTQsImlhdCI6MTYyMjkwNjQ5NH0.
            //                  T5uatcvIgEMQjK3vJmh7YIs8V49rvSjNFzt_Wnf8btM"
        }
    }
}
