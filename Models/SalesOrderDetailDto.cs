public class SalesOrderDetailDto
{
    // Header Information (from tbl_salesOrder)
    public string SoNo { get; set; } = string.Empty;
    public DateTime SoDate { get; set; }
    public string CustAcc { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string SaleMan { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string AccDesc { get; set; } = string.Empty;
    public string Payment { get; set; } = string.Empty;
    public string DeptNo { get; set; } = string.Empty;
    public string JobCode { get; set; } = string.Empty;

    // Transaction / Item Level (from tbl_salesOrdertran)
    public string SoIcode { get; set; } = string.Empty;     // Item Code
    public decimal SoQty { get; set; }
    public decimal SoCost { get; set; }
    public decimal LineTotal { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Idesc { get; set; } = string.Empty;       // Item Description

    // Item Master Details (from tbl_item)
    public string ItemCode { get; set; } = string.Empty;
    public string ItemDesc { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string SGroup { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SCategory { get; set; } = string.Empty;
    public decimal CurStock { get; set; }
    public decimal PriceUnit { get; set; }

    // Address / Customer Details
    public string Address1 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string Address3 { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // Additional useful fields
    public decimal SoFdisc { get; set; }
    public string SoRef { get; set; } = string.Empty;
    public string Custom { get; set; } = string.Empty;   // ONLINE / CONFIRM etc.
    public DateTime? DueDate { get; set; }
}