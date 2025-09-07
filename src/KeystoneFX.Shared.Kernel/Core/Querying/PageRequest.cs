namespace KeystoneFX.Shared.Kernel.Core.Querying;

public class PageRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int Skip => (PageNumber - 1) * PageSize;
    public bool IsPagingEnabled => PageSize > 0;
}
