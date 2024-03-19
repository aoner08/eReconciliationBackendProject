using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Security.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.JWT
{
	public class JwtHelper : ITokenHelper
	{
        public IConfiguration Configuration { get; }
		private TokenOptions _tokenOptions;
		DateTime _accessTokenExpiration;

		public JwtHelper(IConfiguration configuration)
		{
			Configuration = configuration;
			_tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
		}

		public AccessToken CreateToken(User user, List<OperationClaim> operationClaims, int companyId)
		{//token'a erişirse yukardaki user vs bilgilerime erişebilir. giriş yaptığımızda 128 bitlik hashlenmiş bilgidir.
     //bu token'ı postmanda kullanıp işlem yapabiliyoruz o token yoksa yetkin yok hiç bir şey yapamayız varsa sistem ile ilgili işlem yapabiliriz.
			_accessTokenExpiration=DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);//tokenin ne kadar hayatta kalacağını oluşturulduktan itibaren hesaplamasını yapıp ekliyoruz
            var securityKey=SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
			var signinCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
			var jwt = CreateJwtSecurityToken(_tokenOptions, user, signinCredentials, operationClaims, companyId);
			var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
			var token=jwtSecurityTokenHandler.WriteToken(jwt);
			
			return new AccessToken
			{
				Token= token,
				Expiration = _accessTokenExpiration,
				CompanyId=companyId	
			};
		}

		public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials, List<OperationClaim> operationClaims, int companyId)
		{
			var jwt = new JwtSecurityToken(
				issuer: tokenOptions.Issuer,
				audience: tokenOptions.Audience,
				expires: _accessTokenExpiration,
				notBefore: DateTime.Now,
				claims: SetClaims(user, operationClaims, companyId),
				signingCredentials: signingCredentials
				);

			return jwt;
		}

		private IEnumerable<Claim>SetClaims(User user,List<OperationClaim>operationClaims,int companyId)
		{
			var claims=new List<Claim>();
			claims.AddNameIdentitfier(user.Id.ToString());
			claims.AddEmail(user.Email);
			claims.AddName($"{user.Name}");
			claims.AddRoles(operationClaims.Select(c=>c.Name).ToArray());
			claims.AddCompany(companyId.ToString());
			return claims;
		}
	}
}
