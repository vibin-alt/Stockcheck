public class CartItemDto
{
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Oem { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}