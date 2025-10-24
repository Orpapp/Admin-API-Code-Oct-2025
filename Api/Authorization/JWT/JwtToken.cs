using System.IdentityModel.Tokens.Jwt;

namespace Api.Authorization.JWT
{
    /// <summary>
    /// JWT token
    /// </summary>
    public class JwtToken
    {
        private readonly JwtSecurityToken _securityToken;

        internal JwtToken(JwtSecurityToken token)
        {
            _securityToken = token;
        }

        /// <summary>
        /// valid to
        /// </summary>
        public DateTime ValidTo => _securityToken.ValidTo;

        /// <summary>
        /// jwt token value
        /// </summary>
        public string Value => new JwtSecurityTokenHandler().WriteToken(_securityToken);
    }
}
