namespace KeystoneFX.Application.Features.Identity.User.Dtos;

public record UserCreateDto
{
    // Identity essentials
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }

    // Credentials (create-only)
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }

    // Profile
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? NationalCode { get; init; }

    // Authorization (handled by handler, not mapped directly to User)
    public IReadOnlyCollection<string> Roles { get; init; } = [];
    public IReadOnlyCollection<UserClaimInput> Claims { get; init; } = [];
}