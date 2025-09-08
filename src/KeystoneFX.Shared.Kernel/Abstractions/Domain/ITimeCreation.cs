namespace KeystoneFX.Shared.Kernel.Abstractions.Domain;

public interface ITimeCreation
{
    DateTimeOffset CreatedOnUtc { get; set; }
}