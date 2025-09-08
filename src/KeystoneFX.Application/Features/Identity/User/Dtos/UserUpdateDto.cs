namespace KeystoneFX.Application.Features.Identity.User.Dtos;

public sealed record UserUpdateDto
{
    // Identity
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }

    // Profile
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? NationalCode { get; init; }

    // Security toggles (admin-side, optional)
    public bool? TwoFactorEnabled { get; init; }
    public bool? LockoutEnabled { get; init; }

    // Concurrency
    public byte[] RowVersion { get; init; } = [];

    // Authorization (managed in handler)
    public IReadOnlyCollection<string>? Roles { get; init; }
    public IReadOnlyCollection<UserClaimInput>? Claims { get; init; }
}