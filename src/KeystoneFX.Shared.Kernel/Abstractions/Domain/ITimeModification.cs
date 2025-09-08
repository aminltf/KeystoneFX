namespace KeystoneFX.Shared.Kernel.Abstractions.Domain;

public interface ITimeModification
{
    DateTimeOffset? ModifiedAtUtc { get; set; }
}