//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.SqlServer.Server;
//using System.Data;
//using System.Data.Common;

//[ApiController]
//[Route("api/[controller]")]
//public class SalesOrderController : ControllerBase
//{
//    private readonly AppDbContext _context;

//    public SalesOrderController(AppDbContext context)
//    {
//        _context = context;
//    }

//    // ====================== CREATE / EDIT SALES ORDER ======================
//    [HttpPost("save")]
//    public async Task<IActionResult> SaveSalesOrder([FromBody] CreateSalesOrderRequest request)
//    {
//        if (request?.Items == null || !request.Items.Any())
//            return BadRequest(new { success = false, message = "At least one item is required" });

//        bool isEdit = request.Operation?.ToUpper() == "EDIT" || request.SoNo > 0;

//        try
//        {
//            var itemsTable = new DataTable();
//            itemsTable.Columns.Add("Slno", typeof(string));
//            itemsTable.Columns.Add("code", typeof(string));
//            itemsTable.Columns.Add("description", typeof(string));
//            itemsTable.Columns.Add("locn", typeof(string));
//            itemsTable.Columns.Add("unit", typeof(string));
//            itemsTable.Columns.Add("qty", typeof(decimal));
//            itemsTable.Columns.Add("unit price", typeof(decimal));
//            itemsTable.Columns.Add("disc%", typeof(decimal));
//            itemsTable.Columns.Add("Amount", typeof(decimal));
//            itemsTable.Columns.Add("x", typeof(string));
//            itemsTable.Columns.Add("cntrl", typeof(string));
//            itemsTable.Columns.Add("fraction", typeof(decimal));
//            itemsTable.Columns.Add("qno", typeof(decimal));
//            itemsTable.Columns.Add("oem", typeof(string));
//            itemsTable.Columns.Add("vat%", typeof(decimal));
//            itemsTable.Columns.Add("Vatamt", typeof(decimal));
//            itemsTable.Columns.Add("total", typeof(decimal));
//            itemsTable.Columns.Add("UPrice.VIncl", typeof(decimal));
//            itemsTable.Columns.Add("Curstk", typeof(decimal));
//            itemsTable.Columns.Add("Remarks", typeof(string));
//            itemsTable.Columns.Add("Unit cost", typeof(decimal));
//            itemsTable.Columns.Add("BlkPrice", typeof(decimal));

//            foreach (var item in request.Items)
//            {
//                itemsTable.Rows.Add(
//                    item.Slno ?? "1", item.Code, item.Description, item.Locn ?? "", item.Unit ?? "PCS",
//                    item.Qty, item.UnitPrice, item.DiscPercent, item.Amount, "", "", item.Fraction,
//                    0, "", 0, 0, item.Total, 0, 0, item.Remarks ?? "", 0, 0
//                );
//            }

//            using var command = _context.Database.GetDbConnection().CreateCommand() as SqlCommand;
//            command.CommandText = "sp_SalesorderSingle_EditInsert_Vrno_Portal";
//            command.CommandType = CommandType.StoredProcedure;

//            command.Parameters.Add(new SqlParameter("@salesOrderItm", SqlDbType.Structured)
//            {
//                TypeName = "dbo.udtSalesOrder_v3",
//                Value = itemsTable
//            });

//            command.Parameters.AddWithValue("@operation", isEdit ? "EDIT" : "SAVE");
//            command.Parameters.AddWithValue("@so_no", request.SoNo);
//            command.Parameters.AddWithValue("@so_date", request.SoDate);
//            command.Parameters.AddWithValue("@cust_acc", request.CustAcc);
//            command.Parameters.AddWithValue("@jv_num", "00");
//            command.Parameters.AddWithValue("@comments", request.Comments ?? "");
//            command.Parameters.AddWithValue("@sale_man", request.SaleMan ?? "");
//            command.Parameters.AddWithValue("@inv_no", 0);
//            command.Parameters.AddWithValue("@so_status", "");
//            command.Parameters.AddWithValue("@area_code", request.AreaCode ?? "");
//            command.Parameters.AddWithValue("@so_ref", "");
//            command.Parameters.AddWithValue("@so_doc", request.SoNo.ToString());
//            command.Parameters.AddWithValue("@fc", request.Fc ?? "AED");
//            command.Parameters.AddWithValue("@so_amount", request.Items.Sum(i => i.Amount + i.VatAmt));
//            command.Parameters.AddWithValue("@so_fcamt", request.Items.Sum(i => i.Amount + i.VatAmt)); ;
//            command.Parameters.AddWithValue("@so_fcrate", 1.0); 
//            command.Parameters.AddWithValue("@so_disc", request.SoDisc);
//            command.Parameters.AddWithValue("@due_date", request.DueDate ?? request.SoDate);
//            command.Parameters.AddWithValue("@so_fdisc", 0);
//            command.Parameters.AddWithValue("@accdesc", request.AccDesc ?? "");
//            command.Parameters.AddWithValue("@payment", request.Payment ?? "");
//            command.Parameters.AddWithValue("@manuf", "");
//            command.Parameters.AddWithValue("@origin", "");
//            command.Parameters.AddWithValue("@shipment", "");
//            command.Parameters.AddWithValue("@delivery", "");
//            command.Parameters.AddWithValue("@validity", "");
//            command.Parameters.AddWithValue("@packing", "");
//            command.Parameters.AddWithValue("@netwt", "");
//            command.Parameters.AddWithValue("@grosswt", 0);
//            command.Parameters.AddWithValue("@insurance", "");
//            command.Parameters.AddWithValue("@custom", "ONLINE");
//            command.Parameters.AddWithValue("@foot1", "");
//            command.Parameters.AddWithValue("@frdet", "");
//            command.Parameters.AddWithValue("@fruprice", 0);
//            command.Parameters.AddWithValue("@framt",request.Items.Sum(i => i.VatAmt));
//            command.Parameters.AddWithValue("@fob", "Y");
//            command.Parameters.AddWithValue("@qtn_no", "");
//            command.Parameters.AddWithValue("@tel", request.EUser);
//            command.Parameters.AddWithValue("@advance", 0);
//            command.Parameters.AddWithValue("@inv_date", request.SoDate);
//            command.Parameters.AddWithValue("@inv_total", 0);
//            command.Parameters.AddWithValue("@footer1", "");
//            command.Parameters.AddWithValue("@deptno", request.DeptNo);
//            command.Parameters.AddWithValue("@joBcode", "");
//            command.Parameters.AddWithValue("@EUSER", request.EUser);

//            await _context.Database.OpenConnectionAsync();
//            var result = await command.ExecuteScalarAsync();
//            await _context.Database.CloseConnectionAsync();

//            long salesOrderNo = 0;
//            if (result != null && result != DBNull.Value)
//                salesOrderNo = Convert.ToInt64(result);

//            return Ok(new
//            {
//                success = true,
//                message = isEdit
//                    ? $"Sales Order {salesOrderNo} updated successfully!"
//                    : $"Sales Order created successfully! SO No: {salesOrderNo}",
//                salesOrderNo = salesOrderNo
//            });
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("Sales Order Error: " + ex.ToString());
//            return StatusCode(500, new
//            {
//                success = false,
//                message = "Internal server error while saving sales order",
//                error = ex.Message
//            });
//        }
//    }

//    // ====================== LIST SALES ORDERS ======================
//    [HttpGet("view")]
//    public async Task<IActionResult> ViewSalesOrders(
//        [FromQuery] string account,
//        [FromQuery] string filter = "ALL")
//    {
//        if (string.IsNullOrWhiteSpace(account))
//            return BadRequest(new { success = false, message = "Customer account is required" });

//        try
//        {
//            var orders = new List<SalesOrderViewDto>();

//            using var command = _context.Database.GetDbConnection().CreateCommand();
//            command.CommandText = "EXEC sp_PortalView @opmod, @account, @p1, @doc_no";
//            command.CommandType = CommandType.Text;

//            command.Parameters.Add(new SqlParameter("@opmod", "SORD"));
//            command.Parameters.Add(new SqlParameter("@account", account));
//            command.Parameters.Add(new SqlParameter("@p1", filter));
//            command.Parameters.Add(new SqlParameter("@doc_no", DBNull.Value));

//            await _context.Database.OpenConnectionAsync();
//            using var reader = await command.ExecuteReaderAsync();

//            while (await reader.ReadAsync())
//            {
//                var order = new SalesOrderViewDto
//                {
//                    SONo = GetString(reader, "SO NO"),
//                    SODate = GetDateTime(reader, "SO DATE"),
//                    Customer = GetString(reader, "CUSTOMER"),
//                    LPO = GetString(reader, "LPO"),
//                    QTNos = GetString(reader, "QT NOS"),
//                    InvNos = GetString(reader, "INV NOS"),
//                    Amount = GetDecimal(reader, "AMOUNT"),
//                    Status = GetString(reader, "STATUS"),
//                    CustAcc = GetString(reader, "CUST_ACC"),
//                    Salesperson = GetString(reader, "SALESPERSON"),
//                    OrderStatus = GetString(reader, "ORDER STATUS"),
//                    Remarks = GetString(reader, "REMARKS"),
//                    BillStatus = GetString(reader, "Bill Status"),
//                    DONOs = GetString(reader, "DONOS"),
//                    PONo = GetString(reader, "PO_NO"),
//                    GITNos = GetString(reader, "GIT_NOS")
//                };
//                orders.Add(order);
//            }

//            await _context.Database.CloseConnectionAsync();

//            return Ok(new
//            {
//                success = true,
//                message = $"Found {orders.Count} sales orders",
//                filter = filter,
//                data = orders
//            });
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("View Orders Error: " + ex.ToString());
//            return StatusCode(500, new
//            {
//                success = false,
//                message = "Error fetching sales orders",
//                error = ex.Message
//            });
//        }
//    }

//    // ====================== SALES ORDER DETAILS ======================
//    [HttpGet("details")]
//    public async Task<IActionResult> GetSalesOrderDetails(
//        [FromQuery] string account,
//        [FromQuery] string docNo)
//    {
//        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(docNo))
//            return BadRequest(new { success = false, message = "Account and SO Number are required" });

//        try
//        {
//            var details = new List<SalesOrderDetailDto>();

//            using var command = _context.Database.GetDbConnection().CreateCommand();
//            command.CommandText = "EXEC sp_PortalView @opmod, @account, @p1, @doc_no";
//            command.CommandType = CommandType.Text;

//            command.Parameters.Add(new SqlParameter("@opmod", "SORD_DETAILS"));
//            command.Parameters.Add(new SqlParameter("@account", account));
//            command.Parameters.Add(new SqlParameter("@p1", ""));
//            command.Parameters.Add(new SqlParameter("@doc_no", docNo));

//            await _context.Database.OpenConnectionAsync();
//            using var reader = await command.ExecuteReaderAsync();

//            while (await reader.ReadAsync())
//            {
//                var item = new SalesOrderDetailDto
//                {
//                    SoNo = GetString(reader, "so_no"),
//                    SoDate = GetDateTime(reader, "so_date"),
//                    CustAcc = GetString(reader, "cust_acc"),
//                    Comments = GetString(reader, "comments"),
//                    SaleMan = GetString(reader, "sale_man"),
//                    Status = GetString(reader, "so_status"),
//                    Amount = GetDecimal(reader, "so_amount"),
//                    SoIcode = GetString(reader, "so_icode"),
//                    SoQty = GetDecimal(reader, "so_qty"),
//                    Unit = GetString(reader, "unit"),
//                    Idesc = GetString(reader, "idesc"),
//                    LineTotal = GetDecimal(reader, "line_total"),
//                    ItemCode = GetString(reader, "code"),
//                    ItemDesc = GetString(reader, "desc"),
//                    Group = GetString(reader, "group"),
//                    Address1 = GetString(reader, "address1"),
//                    Phone = GetString(reader, "phone"),
//                    Email = GetString(reader, "email")

//                };
//                details.Add(item);
//            }

//            await _context.Database.CloseConnectionAsync();

//            return Ok(new
//            {
//                success = true,
//                message = $"Found {details.Count} item(s) in sales order",
//                soNo = docNo,
//                data = details
//            });
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("Order Details Error: " + ex.ToString());
//            return StatusCode(500, new
//            {
//                success = false,
//                message = "Error fetching order details",
//                error = ex.Message
//            });
//        }
//    }

//    // ==================== Helper Methods ====================
//    private string GetString(DbDataReader reader, string columnName)
//    {
//        try
//        {
//            int ordinal = reader.GetOrdinal(columnName);
//            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetValue(ordinal)?.ToString() ?? "";
//        }
//        catch { return string.Empty; }
//    }

//    private decimal GetDecimal(DbDataReader reader, string columnName)
//    {
//        try
//        {
//            int ordinal = reader.GetOrdinal(columnName);
//            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
//        }
//        catch { return 0; }
//    }

//    private DateTime GetDateTime(DbDataReader reader, string columnName)
//    {
//        try
//        {
//            int ordinal = reader.GetOrdinal(columnName);
//            return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
//        }
//        catch { return DateTime.MinValue; }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

[ApiController]
[Route("api/[controller]")]
public class SalesOrderController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalesOrderController(AppDbContext context)
    {
        _context = context;
    }

    // ====================== CREATE / EDIT SALES ORDER ======================
    [HttpPost("save")]
    public async Task<IActionResult> SaveSalesOrder([FromBody] CreateSalesOrderRequest request)
    {
        if (request?.Items == null || !request.Items.Any())
            return BadRequest(new { success = false, message = "At least one item is required" });

        // Improved Edit detection
        bool isEdit = request.Operation?.Trim().ToUpper() == "EDIT" || request.SoNo > 0;

        try
        {
            var itemsTable = new DataTable();
            itemsTable.Columns.Add("Slno", typeof(string));
            itemsTable.Columns.Add("code", typeof(string));
            itemsTable.Columns.Add("description", typeof(string));
            itemsTable.Columns.Add("locn", typeof(string));
            itemsTable.Columns.Add("unit", typeof(string));
            itemsTable.Columns.Add("qty", typeof(decimal));
            itemsTable.Columns.Add("unit price", typeof(decimal));
            itemsTable.Columns.Add("disc%", typeof(decimal));
            itemsTable.Columns.Add("Amount", typeof(decimal));
            itemsTable.Columns.Add("x", typeof(string));
            itemsTable.Columns.Add("cntrl", typeof(string));
            itemsTable.Columns.Add("fraction", typeof(decimal));
            itemsTable.Columns.Add("qno", typeof(decimal));
            itemsTable.Columns.Add("oem", typeof(string));
            itemsTable.Columns.Add("vat%", typeof(decimal));
            itemsTable.Columns.Add("Vatamt", typeof(decimal));
            itemsTable.Columns.Add("total", typeof(decimal));
            itemsTable.Columns.Add("UPrice.VIncl", typeof(decimal));
            itemsTable.Columns.Add("Curstk", typeof(decimal));
            itemsTable.Columns.Add("Remarks", typeof(string));
            itemsTable.Columns.Add("Unit cost", typeof(decimal));
            itemsTable.Columns.Add("BlkPrice", typeof(decimal));

            foreach (var item in request.Items)
            {
                itemsTable.Rows.Add(
                    item.Slno ?? "1",
                    item.Code,
                    item.Description,
                    item.Locn ?? "",
                    item.Unit ?? "PCS",
                    item.Qty,
                    item.UnitPrice,
                    item.DiscPercent,
                    item.Amount,
                    "", "", item.Fraction, 0, "", 0, 0, item.Total, 0, 0, item.Remarks ?? "", 0, 0
                );
            }

            using var command = _context.Database.GetDbConnection().CreateCommand() as SqlCommand;
            command.CommandText = "sp_SalesorderSingle_EditInsert_Vrno_Portal";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@salesOrderItm", SqlDbType.Structured)
            {
                TypeName = "dbo.udtSalesOrder_v3",
                Value = itemsTable
            });

            // Force correct operation
            command.Parameters.AddWithValue("@operation", isEdit ? "EDIT" : "SAVE");
            command.Parameters.AddWithValue("@so_no", request.SoNo);
            command.Parameters.AddWithValue("@so_date", request.SoDate);
            command.Parameters.AddWithValue("@cust_acc", request.CustAcc);
            command.Parameters.AddWithValue("@jv_num", "00");
            command.Parameters.AddWithValue("@comments", request.Comments ?? "");
            command.Parameters.AddWithValue("@sale_man", request.SaleMan ?? "");
            command.Parameters.AddWithValue("@inv_no", 0);
            command.Parameters.AddWithValue("@so_status", "O");
            command.Parameters.AddWithValue("@area_code", request.AreaCode ?? "");
            command.Parameters.AddWithValue("@so_ref", "");
            command.Parameters.AddWithValue("@so_doc", request.SoNo.ToString());
            command.Parameters.AddWithValue("@fc", request.Fc ?? "AED");
            command.Parameters.AddWithValue("@so_amount", request.Items.Sum(i => i.Amount+i.VatAmt));
            command.Parameters.AddWithValue("@so_fcamt", request.Items.Sum(i => i.Amount + i.VatAmt));
            command.Parameters.AddWithValue("@so_fcrate", 1.0);
            command.Parameters.AddWithValue("@so_disc", request.SoDisc);
            command.Parameters.AddWithValue("@due_date", request.DueDate ?? request.SoDate);
            command.Parameters.AddWithValue("@so_fdisc", 0);
            command.Parameters.AddWithValue("@accdesc", request.AccDesc ?? "");
            command.Parameters.AddWithValue("@payment", request.Payment ?? "");
            command.Parameters.AddWithValue("@manuf", "");
            command.Parameters.AddWithValue("@origin", "");
            command.Parameters.AddWithValue("@shipment", "");
            command.Parameters.AddWithValue("@delivery", "");
            command.Parameters.AddWithValue("@validity", "");
            command.Parameters.AddWithValue("@packing", "");
            command.Parameters.AddWithValue("@netwt", "");
            command.Parameters.AddWithValue("@grosswt", 0);
            command.Parameters.AddWithValue("@insurance", "");
            command.Parameters.AddWithValue("@custom", "ONLINE");
            command.Parameters.AddWithValue("@foot1", "");
            command.Parameters.AddWithValue("@frdet", "");
            command.Parameters.AddWithValue("@fruprice", 0);
            command.Parameters.AddWithValue("@framt", request.Items.Sum(i =>  i.VatAmt));
            command.Parameters.AddWithValue("@fob", "Y");
            command.Parameters.AddWithValue("@qtn_no", "");
            command.Parameters.AddWithValue("@tel", request.EUser);
            command.Parameters.AddWithValue("@advance", 0);
            command.Parameters.AddWithValue("@inv_date", request.SoDate);
            command.Parameters.AddWithValue("@inv_total", 0);
            command.Parameters.AddWithValue("@footer1", "");
            command.Parameters.AddWithValue("@deptno", request.DeptNo);
            command.Parameters.AddWithValue("@joBcode", "");
            command.Parameters.AddWithValue("@EUSER", request.EUser);

            await _context.Database.OpenConnectionAsync();
            var result = await command.ExecuteScalarAsync();
            await _context.Database.CloseConnectionAsync();

            long salesOrderNo = 0;
            if (result != null && result != DBNull.Value)
                salesOrderNo = Convert.ToInt64(result);

            return Ok(new
            {
                success = true,
                message = isEdit
                    ? $"Sales Order {salesOrderNo} updated successfully!"
                    : $"Sales Order created successfully! SO No: {salesOrderNo}",
                salesOrderNo = salesOrderNo,
                isEdit = isEdit
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Sales Order Error: " + ex.ToString());
            return StatusCode(500, new
            {
                success = false,
                message = isEdit ? "Failed to update sales order" : "Failed to create sales order",
                error = ex.Message
            });
        }
    }

    // ====================== LIST SALES ORDERS ======================
    [HttpGet("view")]
    public async Task<IActionResult> ViewSalesOrders(
        [FromQuery] string account,
        [FromQuery] string filter = "ALL")
    {
        if (string.IsNullOrWhiteSpace(account))
            return BadRequest(new { success = false, message = "Customer account is required" });

        try
        {
            var orders = new List<SalesOrderViewDto>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "EXEC sp_PortalView @opmod, @account, @p1, @doc_no";
            command.CommandType = CommandType.Text;

            command.Parameters.Add(new SqlParameter("@opmod", "SORD"));
            command.Parameters.Add(new SqlParameter("@account", account));
            command.Parameters.Add(new SqlParameter("@p1", filter));
            command.Parameters.Add(new SqlParameter("@doc_no", DBNull.Value));

            await _context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var order = new SalesOrderViewDto
                {
                    SONo = GetString(reader, "SO NO"),
                    SODate = GetDateTime(reader, "SO DATE"),
                    Customer = GetString(reader, "CUSTOMER"),
                    LPO = GetString(reader, "LPO"),
                    QTNos = GetString(reader, "QT NOS"),
                    InvNos = GetString(reader, "INV NOS"),
                    Amount = GetDecimal(reader, "AMOUNT"),
                    Status = GetString(reader, "STATUS"),
                    CustAcc = GetString(reader, "CUST_ACC"),
                    Salesperson = GetString(reader, "SALESPERSON"),
                    OrderStatus = GetString(reader, "ORDER STATUS"),
                    Remarks = GetString(reader, "REMARKS"),
                    BillStatus = GetString(reader, "Bill Status"),
                    DONOs = GetString(reader, "DONOS"),
                    PONo = GetString(reader, "PO_NO"),
                    GITNos = GetString(reader, "GIT_NOS")
                };
                orders.Add(order);
            }

            await _context.Database.CloseConnectionAsync();

            return Ok(new
            {
                success = true,
                message = $"Found {orders.Count} sales orders",
                filter = filter,
                data = orders
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("View Orders Error: " + ex.ToString());
            return StatusCode(500, new
            {
                success = false,
                message = "Error fetching sales orders",
                error = ex.Message
            });
        }
    }

    // ====================== SALES ORDER DETAILS ======================
    [HttpGet("details")]
    public async Task<IActionResult> GetSalesOrderDetails(
        [FromQuery] string account,
        [FromQuery] string docNo)
    {
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(docNo))
            return BadRequest(new { success = false, message = "Account and SO Number are required" });

        try
        {
            var details = new List<SalesOrderDetailDto>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "EXEC sp_PortalView @opmod, @account, @p1, @doc_no";
            command.CommandType = CommandType.Text;

            command.Parameters.Add(new SqlParameter("@opmod", "SORD_DETAILS"));
            command.Parameters.Add(new SqlParameter("@account", account));
            command.Parameters.Add(new SqlParameter("@p1", ""));
            command.Parameters.Add(new SqlParameter("@doc_no", docNo));

            await _context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new SalesOrderDetailDto
                {
                    SoNo = GetString(reader, "so_no"),
                    SoDate = GetDateTime(reader, "so_date"),
                    CustAcc = GetString(reader, "cust_acc"),
                    Comments = GetString(reader, "comments"),
                    SaleMan = GetString(reader, "sale_man"),
                    Status = GetString(reader, "so_status"),
                    Amount = GetDecimal(reader, "so_amount"),
                    SoIcode = GetString(reader, "so_icode"),
                    SoQty = GetDecimal(reader, "so_qty"),
                    Unit = GetString(reader, "unit"),
                    Idesc = GetString(reader, "idesc"),
                    LineTotal = GetDecimal(reader, "line_total"),
                    ItemCode = GetString(reader, "code"),
                    ItemDesc = GetString(reader, "desc"),
                    Group = GetString(reader, "group"),
                    Address1 = GetString(reader, "address1"),
                    Phone = GetString(reader, "phone"),
                    Email = GetString(reader, "email")
                };
                details.Add(item);
            }

            await _context.Database.CloseConnectionAsync();

            return Ok(new
            {
                success = true,
                message = $"Found {details.Count} item(s) in sales order",
                soNo = docNo,
                data = details
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Order Details Error: " + ex.ToString());
            return StatusCode(500, new
            {
                success = false,
                message = "Error fetching order details",
                error = ex.Message
            });
        }
    }

    // ==================== Helper Methods ====================
    private string GetString(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetValue(ordinal)?.ToString() ?? "";
        }
        catch { return string.Empty; }
    }

    private decimal GetDecimal(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }
        catch { return 0; }
    }

    private DateTime GetDateTime(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
        }
        catch { return DateTime.MinValue; }
    }
}