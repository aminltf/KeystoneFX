namespace KeystoneFX.Application.Features.Identity.Role.Dtos;

public sealed record RoleUpdateDto
{
    public string? DisplayName { get; init; }
    public IReadOnlyCollection<RoleClaimInput>? Claims { get; init; }
    public byte[] RowVersion { get; init; } = [];
}