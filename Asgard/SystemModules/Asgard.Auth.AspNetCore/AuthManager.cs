using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Asgard.Abstract.Models.AsgardUserInfo;
using Asgard.Extends.Json;
using Asgard.Tools;

using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Asgard.Auth.AspNetCore
{
    public class AuthManager
    {
        private readonly byte[] _jwtKey;
        private readonly JwtSecurityTokenHandler _hander = new();
        private readonly TokenValidationParameters _validationParams;
        private readonly SigningCredentials _signingCredentials;
        private readonly string _audience;
        private readonly string _issuer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="jwtKey">密钥RSA512</param>
        /// <param name="issuer">颁发人</param>
        /// <param name="audience">听众</param> 
        public AuthManager(string jwtKey, string issuer, string audience)
        {
            IdentityModelEventSource.ShowPII = true;
            _audience = audience;
            _issuer = issuer;
            _jwtKey = Convert.FromBase64String(jwtKey);
            var key = new SymmetricSecurityKey(_jwtKey);
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            _validationParams = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ConfigurationManager = null,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(_jwtKey),
                ClockSkew = TimeSpan.FromSeconds(20), // 可选：允许 nbf/exp 最多偏移 20 秒 
            };
        }


        /// <summary>
        /// 解密获取某一个用户信息
        /// </summary>
        public bool TryGetUserInfo(string rawToken, out UserInfo? userInfo, out string? jti, out TokenType type)
        {
            jti = null;
            userInfo = null;
            type = TokenType.InvalidToken;
            try
            {
                var res = _hander.ValidateToken(rawToken, _validationParams, out var tokenInfo);
                if (tokenInfo is not JwtSecurityToken token)
                {
                    return false;
                }
                jti = token.Id;
                var rawUID = token.Claims.FirstOrDefault(item => item.Type == "UID");
                var rawDetailInfo = token.Claims.FirstOrDefault(item => item.Type == "detail");
                var tokenTypeStr = token.Claims.FirstOrDefault(item => item.Type == "type");
                if (tokenTypeStr is null || !int.TryParse(tokenTypeStr.Value, out var tokenType) || !Enum.IsDefined(typeof(TokenType), tokenType))
                {
                    return false;
                }

                type = (TokenType)tokenType;
                if (rawDetailInfo is null || string.IsNullOrWhiteSpace(rawDetailInfo.ToString()))
                {
                    return false;
                }
                var info = BaseEncryptionTools.DecryptStringFromStr_Aes(rawDetailInfo.Value.ToString());
                if (string.IsNullOrWhiteSpace(info))
                {
                    return false;
                }

                userInfo = Convert.FromBase64String(info).GetObject<UserInfo>(JsonCompressLevel.Normal);
                if (userInfo is null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 尝试创建一个Token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="token"></param>
        /// <param name="delayWork"></param>
        /// <param name="expire"></param> 
        /// <returns></returns>
        public bool TryCreateToken(UserInfo userInfo, out string? token, DateTime? delayWork, DateTime? expire)
        {
            token = string.Empty;
            try
            {
                var claims = CreateUserClaims(userInfo, TokenType.AccessToken);
                if (claims is null)
                {
                    return false;
                }

                var jwtToken = new JwtSecurityToken(issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    notBefore: delayWork ?? DateTime.Now,
                    expires: expire ?? DateTime.Now.AddHours(5),
                    signingCredentials: _signingCredentials);
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// 创建一个超长期的刷新Token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="token"></param>
        /// <param name="jti"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public bool TryCreateRefreshToken(UserInfo userInfo, out string token, out string jti, DateTime? expires = null)
        {
            jti = token = string.Empty;
            try
            {
                var claims = CreateUserClaims(userInfo, TokenType.RefreshToken);
                if (claims is null)
                {
                    return false;
                }

                var jwtToken = new JwtSecurityToken(issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: expires ?? DateTime.Now.AddMonths(1),
                    signingCredentials: _signingCredentials);
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                jti = jwtToken.Id;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建一个用户描述信息列表
        /// </summary> 
        private static List<Claim>? CreateUserClaims(UserInfo userInfo, TokenType type)
        {
            var claims = new List<Claim>(4) {
                    new Claim("UID", userInfo.UID.ToString()),
                    new Claim("jti", Guid.NewGuid().ToString("N")) ,
                     new Claim("type",((int)type).ToString()),
                };
            var rawBytes = userInfo.GetBytes(JsonCompressLevel.Normal);
            if (rawBytes is null)
            {
                return default;
            }
            var info = BaseEncryptionTools.EncryptStringToString_Aes(Convert.ToBase64String(rawBytes));
            claims.Add(new Claim("detail", info ?? ""));
            return claims;
        }
    }
}
