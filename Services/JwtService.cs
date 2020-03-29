using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Live.Core;
using Live.Extensions;
using Live.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Live.Services
{
    public  class JwtService : IJwtService
{

      private readonly TokenParameters _tokenParameters;

      public JwtService(TokenParameters tokenParameters)
      {
          _tokenParameters = tokenParameters;
      }

     public JwtDto CreateToken (Guid UserId, string role)
      {
          var now = DateTime.UtcNow;
          var expires = now.AddDays(_tokenParameters.ExpiryDays);
          var claims = new List<Claim>()
          {
              new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim (JwtRegisteredClaimNames.Sub, UserId.ToString()),
              new Claim (JwtRegisteredClaimNames.UniqueName, UserId.ToString()),
              new Claim (ClaimTypes.Role, role),
              new Claim(ClaimTypes.Expired, expires.ToTimestamp().ToString() ),
              new Claim (ClaimTypes.Name, UserId.ToString()),
              new Claim (JwtRegisteredClaimNames.Iat, now.ToTimestamp().ToString()) 
          };

          var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8
          .GetBytes(_tokenParameters.SigningKey)), SecurityAlgorithms.HmacSha256);

        string EncodedToken = 
            new  JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
             issuer: _tokenParameters.Issuer,
             claims: claims,
             expires: expires,
             signingCredentials: signingCredentials));

         return new JwtDto()
         {
             Token = EncodedToken,
             Expire = expires, 
             Roles = role,

         };
      }
        
    }
}