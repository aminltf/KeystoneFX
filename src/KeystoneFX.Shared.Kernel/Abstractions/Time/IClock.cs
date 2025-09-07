namespace KeystoneFX.Shared.Kernel.Abstractions.Time;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}