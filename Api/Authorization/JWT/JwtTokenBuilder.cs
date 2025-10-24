using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Shared.Model.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace Api.Authorization.JWT
{
    /// <summary>
    /// JWT token builder
    /// </summary>
    public class JwtTokenBuilder
    {
        private SecurityKey? _securityKey = null;
        private string _subject = "";
        private string _isUser = "";
        private string _audience = "";
        private readonly Dictionary<string, string> _claims = new();
        private int _expiryInMinutes = 2880;

        /// <summary>
        /// add jwt sqcurity key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public JwtTokenBuilder AddSecurityKey(SecurityKey key)
        {
            this._securityKey = key;
            return this;
        }

        /// <summary>
        /// add subject
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public JwtTokenBuilder AddSubject(string subject)
        {
            this._subject = subject;
            return this;
        }

        /// <summary>
        /// add user 
        /// </summary>
        /// <param name="isUser"></param>
        /// <returns></returns>
        public JwtTokenBuilder AddIsUser(string isUser)
        {
            this._isUser = isUser;
            return this;
        }

        /// <summary>
        /// add audience
        /// </summary>
        /// <param name="audience"></param>
        /// <returns></returns>
        public JwtTokenBuilder AddAudience(string audience)
        {
            this._audience = audience;
            return this;
        }

        /// <summary>
        /// add claims
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public JwtTokenBuilder AddClaim(string type, string value)
        {
            this._claims.Add(type, value);
            return this;
        }

        /// <summary>
        /// add claims
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public JwtTokenBuilder AddClaim(Dictionary<string, string> claims)
        {
            this._claims.Union(claims);
            return this;
        }

        /// <summary>
        /// add expiry
        /// </summary>
        /// <param name="expiryInMinutes"></param>
        /// <returns></returns>
        public JwtTokenBuilder AddExpiry(int expiryInMinutes)
        {
            this._expiryInMinutes = expiryInMinutes;
            return this;
        }

        /// <summary>
        /// build token
        /// </summary>
        /// <returns></returns>
        public JwtToken Build()
        {
            EnsureArguments();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,this._subject),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            }
            .Union(this._claims.Select(item => new Claim(item.Key, item.Value)));

            var token = new JwtSecurityToken
                (
                issuer: this._isUser,
                audience: this._audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(this._expiryInMinutes),
                notBefore: DateTime.Now,
                signingCredentials: new SigningCredentials(this._securityKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtToken(token);
        }

        private void EnsureArguments()
        {
            if (this._securityKey == null)
                throw new ArgumentNullException("Security Key");

            if (string.IsNullOrEmpty(this._subject))
                throw new ArgumentNullException("Subject");

            if (string.IsNullOrEmpty(this._isUser))
                throw new ArgumentNullException("isUser");

            if (string.IsNullOrEmpty(this._audience))
                throw new ArgumentNullException("audience");
        }

        /// <summary>
        /// Get jwt token after login success
        /// </summary>
        /// <param name="jwtTokenSettings"></param>
        /// <param name="userId"></param>
        /// <param name="UserRole"></param>
        /// <returns>JWT token</returns>
        public JwtToken GetToken(JwtTokenSettings jwtTokenSettings, long userId)
        {
            JwtToken token = new JwtTokenBuilder()
                .AddSubject(string.IsNullOrEmpty(jwtTokenSettings.Subject) ? "" : jwtTokenSettings.Subject)
                .AddSecurityKey(JwtSecurityKey.Create(string.IsNullOrEmpty(jwtTokenSettings.Secret) ? "" : jwtTokenSettings.Secret))
                .AddIsUser(string.IsNullOrEmpty(jwtTokenSettings.IsUser) ? "" : jwtTokenSettings.IsUser)
                .AddAudience(string.IsNullOrEmpty(jwtTokenSettings.Audience) ? "" : jwtTokenSettings.Audience)
                .AddClaim("userId", userId.ToString())
                .AddExpiry(jwtTokenSettings.Expiry)
                .Build();

            return token;
        }
  
    }
}