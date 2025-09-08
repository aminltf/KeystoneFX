using KeystoneFX.Shared.Kernel.Core.Domain.Base;

namespace KeystoneFX.Domain.Entities;

public class Product : FullyTrackedEntityBase<Guid, Guid>
{
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }

    public Product() { }

    public Product(string productName, decimal price)
    {
        ProductName = productName;
        Price = price;
    }
}