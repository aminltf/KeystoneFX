namespace KeystoneFX.Application.Features.Identity.Role.Dtos;

public sealed record RoleCreateDto
{
    public required string Name { get; init; }
    public string? DisplayName { get; init; }
    public IReadOnlyCollection<RoleClaimInput> Claims { get; init; } = [];
}