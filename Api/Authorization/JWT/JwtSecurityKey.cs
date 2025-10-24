
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Authorization.JWT
{
    /// <summary>
    /// jwt token security key
    /// </summary>
    public class JwtSecurityKey
    {
        /// <summary>
        /// Create symmetric key
        /// </summary>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }
}
