public class CreateSalesOrderRequest
{
    public string Operation { get; set; } = "SAVE";
    public decimal SoNo { get; set; } = 0;
    public DateTime SoDate { get; set; } = DateTime.UtcNow;
    public string CustAcc { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string SaleMan { get; set; } = string.Empty;
    public string AreaCode { get; set; } = string.Empty;
    public string Fc { get; set; } = "AED";
    public decimal SoDisc { get; set; } = 0;
    public DateTime? DueDate { get; set; }
    public string AccDesc { get; set; } = string.Empty;
    public string Payment { get; set; } = string.Empty;
    public string DeptNo { get; set; } = "HO";
    public string EUser { get; set; } = "ADMIN";
    public string framt { get; set; }

    public List<SalesOrderItemDto> Items { get; set; } = new();
}