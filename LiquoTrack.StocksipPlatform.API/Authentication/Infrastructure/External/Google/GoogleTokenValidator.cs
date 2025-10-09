using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google
{
    /// <summary>
    /// Custom Google token validator
    /// </summary>
    /// <remarks>
    /// This class is used to validate the Google token.
    /// </remarks>
    public class CustomGoogleTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public bool CanValidateToken => true;
        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
        public bool CanReadToken(string securityToken) => true;

        public CustomGoogleTokenValidator()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="securityToken">The token to validate.</param>
        /// <param name="validationParameters">The validation parameters.</param>
        /// <param name="validatedToken">The validated token.</param>
        /// <returns>The claims principal.</returns>
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            if (string.IsNullOrWhiteSpace(securityToken))
            {
                throw new ArgumentNullException(nameof(securityToken));
            }

            ArgumentNullException.ThrowIfNull(validationParameters);

            try
            {
                var jwt = _tokenHandler.ReadJwtToken(securityToken);

                var validationParametersClone = validationParameters.Clone();
                
                validationParametersClone.ValidateIssuerSigningKey = false;
                validationParametersClone.RequireSignedTokens = false;

                JwtSecurityToken validatedJwt = null;
                validationParametersClone.SignatureValidator = (token, parameters) =>
                {
                    validatedJwt = new JwtSecurityToken(token);
                    return validatedJwt;
                };

                var principal = _tokenHandler.ValidateToken(securityToken, validationParametersClone, out var validatedTokenOut);
                validatedToken = validatedJwt ?? validatedTokenOut;

                var claims = new List<Claim>();
                foreach (var claim in jwt.Claims)
                {
                    string value = claim.Value;
                    

                    if ((claim.Type == "sub" || claim.Type == "email" || claim.Type == "name") && 
                        value.StartsWith("[") && value.EndsWith("]"))
                    {
                        value = value.Trim('[', ']')
                                   .Split(',')
                                   .FirstOrDefault()
                                   ?.Trim('\"', ' ', '\'') 
                                   ?? value;
                    }

                    claims.Add(new Claim(claim.Type, value, claim.ValueType, claim.Issuer, claim.OriginalIssuer));
                }

                var identity = new ClaimsIdentity(
                    claims,
                    "Google",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                validatedToken = validatedJwt ?? validatedTokenOut;
                
                return new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                validatedToken = null;
                throw new SecurityTokenValidationException("Invalid token", ex);
            }
        }
    }
}
