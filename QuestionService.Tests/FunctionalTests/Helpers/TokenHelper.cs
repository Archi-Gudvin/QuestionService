using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuestionService.Domain.Dtos.ExternalEntity;

namespace QuestionService.Tests.FunctionalTests.Helpers;

internal static class TokenHelper
{
    private const string Audience = "TestAudience";
    private const string Issuer = "TestIssuer";
    private const string Kid = "test-key-id";

    private static readonly RsaSecurityKey PrivateKey;

    private static readonly string PublicJwk;

    static TokenHelper()
    {
        var rsa = RSA.Create();
        PrivateKey = new RsaSecurityKey(rsa);
        var publicKey = new RsaSecurityKey(rsa.ExportParameters(false));

        PrivateKey.KeyId = Kid;

        var rsaParams = publicKey.Parameters;

        var modulus = Base64UrlEncode(rsaParams.Modulus!);
        var exponent = Base64UrlEncode(rsaParams.Exponent!);

        var jwks = new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA",
                    use = "sig",
                    alg = "RS256",
                    kid = Kid,
                    n = modulus,
                    e = exponent
                }
            }
        };

        PublicJwk = JsonConvert.SerializeObject(jwks);
    }

    public static string GetJwk()
    {
        return PublicJwk;
    }

    public static string GetAudience()
    {
        return Audience;
    }

    public static string GetIssuer()
    {
        return Issuer;
    }

    public static string GetRsaToken(string username, long userId, IEnumerable<RoleDto> roles)
    {
        Claim[] claims =
        [
            ..roles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToArray(),
            new(JwtRegisteredClaimNames.PreferredUsername, username),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, "testEmail@test.com")
        ];

        var tokenString = claims.GetRsaTokenFromClaims();

        return tokenString;
    }

    private static string GetRsaTokenFromClaims(this IEnumerable<Claim> claims)
    {
        var header = new JwtHeader(new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256));
        var payload = new JwtPayload(
            Issuer,
            Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(15)
        );

        var token = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}