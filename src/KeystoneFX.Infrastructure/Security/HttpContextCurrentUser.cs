using KeystoneFX.Shared.Kernel.Abstractions.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace KeystoneFX.Infrastructure.Security;

public sealed class HttpContextCurrentUser<TUserKey> : ICurrentUser<TUserKey>
    where TUserKey : struct, IEquatable<TUserKey>
{
    private readonly IHttpContextAccessor _http;

    public HttpContextCurrentUser(IHttpContextAccessor http) => _http = http;

    public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public TUserKey? UserId
    {
        get
        {
            var p = _http.HttpContext?.User;
            if (p is null) return null;

            var idValue = GetFirstClaimValue(p,
                ClaimTypes.NameIdentifier, "sub", "uid", "id", "user_id");

            if (string.IsNullOrWhiteSpace(idValue)) return null;
            return TryParseKey(idValue, out var key) ? key : (TUserKey?)null;
        }
    }

    public string? UserName
    {
        get
        {
            var p = _http.HttpContext?.User;
            return GetFirstClaimValue(p,
                "name", ClaimTypes.Name, "preferred_username", "username");
        }
    }

    public string? Email
    {
        get
        {
            var p = _http.HttpContext?.User;
            return GetFirstClaimValue(p, ClaimTypes.Email, "email");
        }
    }

    public IEnumerable<string> Roles =>
        (_http.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>())
        .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
        .SelectMany(c => c.Type == "roles"
            ? c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : new[] { c.Value })
        .Distinct(StringComparer.OrdinalIgnoreCase);

    #region Helpers
    private static string? GetFirstClaimValue(ClaimsPrincipal? principal, params string[] types)
    {
        if (principal is null) return null;
        foreach (var t in types)
        {
            var val = principal.FindFirst(t)?.Value;
            if (!string.IsNullOrWhiteSpace(val)) return val;
        }
        return null;
    }

    private static bool TryParseKey(string value, out TUserKey key)
    {
        var t = typeof(TUserKey);

        if (t == typeof(Guid) && Guid.TryParse(value, out var g))
        { key = (TUserKey)(object)g; return true; }
        if (t == typeof(int) && int.TryParse(value, out var i))
        { key = (TUserKey)(object)i; return true; }
        if (t == typeof(long) && long.TryParse(value, out var l))
        { key = (TUserKey)(object)l; return true; }
        if (t == typeof(short) && short.TryParse(value, out var s))
        { key = (TUserKey)(object)s; return true; }

        try
        {
            var conv = System.ComponentModel.TypeDescriptor.GetConverter(t);
            if (conv.CanConvertFrom(typeof(string)))
            {
                var converted = conv.ConvertFromInvariantString(value);
                if (converted is not null)
                { key = (TUserKey)converted; return true; }
            }
        }
        catch
        {

        }

        key = default;
        return false;
    }
    #endregion
}