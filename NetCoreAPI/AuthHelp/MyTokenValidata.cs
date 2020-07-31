using Common.Cache;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Authorization
{
    public class MyTokenValidata : ISecurityTokenValidator
    {
        private readonly ICacheManager _redisCache;
        public MyTokenValidata(ICacheManager redisCacheManager)
        {
            _redisCache = redisCacheManager;
        }
        //判断当前token是否有值
        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; }//顾名思义是验证token的最大bytes

        public bool CanReadToken(string securityToken)
        {
            return true;
        }
        ///验证securityToken
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            //获取 JwtSecurityToken
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(securityToken);

            validatedToken = jwt;
            if (!_redisCache.Exists(securityToken))
            {
                return null;
            }
            
            //前往验证
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(securityToken, validationParameters, out validatedToken);
            return principal;
        }

    }
}
