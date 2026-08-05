using System.Security.Claims;

namespace QuestionService.Domain.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new InvalidOperationException($"{ClaimTypes.NameIdentifier} claim missing.");

        return long.Parse(value);
    }

    public static bool TryGetUserId(this ClaimsPrincipal user, out long? userId)
    {
        userId = null;
        if (!user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            return false;

        userId = user.GetUserId();
        return true;
    }
}
