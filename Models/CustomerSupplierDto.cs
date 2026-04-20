namespace StkPortal.Models
{
    public class CustomerSupplierDto
    {
        public string Mod { get; set; } = string.Empty;           // 'I' or 'M'
        public string Cscode { get; set; } = string.Empty;
        public string AcCode { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CsDes { get; set; } = string.Empty;
        public decimal BalanceOpen { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public string Fc { get; set; } = string.Empty;
        public decimal Fdr { get; set; }
        public decimal Fcr { get; set; }
        public string Cmpcode { get; set; } = string.Empty;
        public string Divcode { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string Adr1 { get; set; } = string.Empty;
        public string Adr2 { get; set; } = string.Empty;
        public string Adr3 { get; set; } = string.Empty;
        public string Adr4 { get; set; } = string.Empty;
        public string Adr5 { get; set; } = string.Empty;
        public string Tel { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Salesman { get; set; } = string.Empty;
        public string Areacode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public decimal Crlimit { get; set; }
        public int Duedays { get; set; }
        public string Terms { get; set; } = string.Empty;
        public string Crmethod { get; set; } = string.Empty;
        public string Cntrlcode { get; set; } = string.Empty;
        public string Avtive { get; set; } = string.Empty;
        public string Colourcode { get; set; } = string.Empty;
        public string PriceCat { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Vatregno { get; set; } = string.Empty;
        public string Shippingadr { get; set; } = string.Empty;
        public string CreateUser { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public decimal Margin { get; set; }
        public int Doduedays { get; set; }
        public string UserName { get; set; } = string.Empty;   // @user_name
        public string PassWord { get; set; } = string.Empty;   // @pass_word
    }


}
