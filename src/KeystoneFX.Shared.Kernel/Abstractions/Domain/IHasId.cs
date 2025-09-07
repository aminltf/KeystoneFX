namespace KeystoneFX.Shared.Kernel.Abstractions.Domain;

public interface IHasId<TKey> where TKey : IEquatable<TKey>
{
    TKey Id { get; }
}