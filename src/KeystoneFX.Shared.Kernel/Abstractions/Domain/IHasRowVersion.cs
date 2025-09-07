namespace KeystoneFX.Shared.Kernel.Abstractions.Domain;

public interface IHasRowVersion
{
    byte[] RowVersion { get; set; }
}