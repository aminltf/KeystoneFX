using System.Security.Claims;

namespace KeystoneFX.Application.Common.Security;

public class ClaimEqualityComparer : IEqualityComparer<Claim>
{
    private readonly StringComparer _typeComparer;
    private readonly StringComparer _valueComparer;
    private readonly bool _compareIssuer;
    private readonly StringComparer _issuerComparer;
    private readonly bool _compareValueType;
    private readonly StringComparer _valueTypeComparer;

    private ClaimEqualityComparer(
        StringComparer typeComparer,
        StringComparer valueComparer,
        bool compareIssuer,
        StringComparer issuerComparer,
        bool compareValueType,
        StringComparer valueTypeComparer)
    {
        _typeComparer = typeComparer ?? StringComparer.Ordinal;
        _valueComparer = valueComparer ?? StringComparer.Ordinal;
        _compareIssuer = compareIssuer;
        _issuerComparer = issuerComparer ?? StringComparer.Ordinal;
        _compareValueType = compareValueType;
        _valueTypeComparer = valueTypeComparer ?? StringComparer.Ordinal;
    }

    public static ClaimEqualityComparer TypeAndValue { get; }
        = new(StringComparer.Ordinal, StringComparer.Ordinal, false, StringComparer.Ordinal, false, StringComparer.Ordinal);

    public static ClaimEqualityComparer TypeAndValueIgnoreCase { get; }
        = new(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase, false, StringComparer.OrdinalIgnoreCase, false, StringComparer.OrdinalIgnoreCase);

    public static ClaimEqualityComparer TypeValueIssuer { get; }
        = new(StringComparer.Ordinal, StringComparer.Ordinal, true, StringComparer.Ordinal, false, StringComparer.Ordinal);

    public static ClaimEqualityComparer TypeValueIssuerIgnoreCase { get; }
        = new(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase, true, StringComparer.OrdinalIgnoreCase, false, StringComparer.OrdinalIgnoreCase);

    public static ClaimEqualityComparer Create(
        StringComparer typeComparer,
        StringComparer valueComparer,
        bool compareIssuer = false,
        StringComparer? issuerComparer = null,
        bool compareValueType = false,
        StringComparer? valueTypeComparer = null)
        => new(typeComparer,
               valueComparer,
               compareIssuer,
               issuerComparer ?? StringComparer.Ordinal,
               compareValueType,
               valueTypeComparer ?? StringComparer.Ordinal);

    public bool Equals(Claim? x, Claim? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        if (!_typeComparer.Equals(x.Type, y.Type)) return false;
        if (!_valueComparer.Equals(x.Value, y.Value)) return false;

        if (_compareIssuer &&
            !_issuerComparer.Equals(x.Issuer ?? string.Empty, y.Issuer ?? string.Empty))
            return false;

        if (_compareValueType &&
            !_valueTypeComparer.Equals(x.ValueType ?? string.Empty, y.ValueType ?? string.Empty))
            return false;

        return true;
    }

    public int GetHashCode(Claim obj)
    {
        var hash = new HashCode();
        hash.Add(obj.Type, _typeComparer);
        hash.Add(obj.Value, _valueComparer);

        if (_compareIssuer)
            hash.Add(obj.Issuer ?? string.Empty, _issuerComparer);

        if (_compareValueType)
            hash.Add(obj.ValueType ?? string.Empty, _valueTypeComparer);

        return hash.ToHashCode();
    }
}