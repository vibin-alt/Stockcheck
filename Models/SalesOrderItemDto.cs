public class SalesOrderItemDto
{
    public string Slno { get; set; } = "1";
    public string Code { get; set; } = string.Empty;           // Part Number (Very Important)
    public string Description { get; set; } = string.Empty;
    public string Locn { get; set; } = "A";                    // Default location
    public string Unit { get; set; } = "PCS";
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscPercent { get; set; } = 0;
    public decimal Amount { get; set; }
    public string? X { get; set; } = "";
    public string? Cntrl { get; set; } = "";
    public decimal Fraction { get; set; } = 1;
    public decimal? Qno { get; set; } = 0;
    public string? Oem { get; set; } = "";
    public decimal VatPercent { get; set; } = 0;
    public decimal VatAmt { get; set; } = 0;
    public decimal Total { get; set; } = 0;
    public decimal UPriceVIncl { get; set; } = 0;
    public decimal Curstk { get; set; } = 0;
    public string? Remarks { get; set; } = "";
    public decimal UnitCost { get; set; } = 0;
    public decimal BlkPrice { get; set; } = 0;
}