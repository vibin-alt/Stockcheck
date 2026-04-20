public class ItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Oem { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Specification { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal StockQty { get; set; }
    public string Picture { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    //public string? MoreRef1 { get; set; }
    //public string? MoreRef2 { get; set; }
    //public string? MoreRef3 { get; set; } = string.Empty;
    public string Substitute { get; set; } = string.Empty;
}