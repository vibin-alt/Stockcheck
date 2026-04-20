using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    // ====================== GET USER CART ======================
    [HttpGet("view")]
    public async Task<IActionResult> GetCart([FromQuery] string custAcc)
    {
        if (string.IsNullOrWhiteSpace(custAcc))
            return BadRequest(new { success = false, message = "Customer account is required" });

        try
        {
            var cartItems = new List<CartItemDto>();

            using var command = _context.Database.GetDbConnection().CreateCommand() as SqlCommand;
            command.CommandText = @"
                SELECT 
                    c.itemId, 
                    c.quantity, 
                    c.unitPrice, 
                    i.[desc] as description, 
                    i.price_unit as price, 
                    i.batch as oem, 
                    i.categ as category
                FROM CartItems c
                LEFT JOIN tbl_item i 
                    ON c.itemId COLLATE Arabic_CI_AI = i.code COLLATE Arabic_CI_AI
                WHERE c.custAcc = @custAcc
                ORDER BY c.updatedAt DESC";

            command.Parameters.Add(new SqlParameter("@custAcc", custAcc));

            await _context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                cartItems.Add(new CartItemDto
                {
                    ItemId = GetString(reader, "itemId"),
                    Quantity = GetInt(reader, "quantity"),
                    UnitPrice = GetDecimal(reader, "unitPrice"),
                    Description = GetString(reader, "description"),
                    Price = GetDecimal(reader, "price"),
                    Oem = GetString(reader, "oem"),
                    Category = GetString(reader, "category")
                });
            }

            await _context.Database.CloseConnectionAsync();

            return Ok(new
            {
                success = true,
                data = cartItems
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Get Cart Error: " + ex.ToString());
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    // ====================== ADD / UPDATE ITEM ======================
    [HttpPost("save")]
    public async Task<IActionResult> SaveCartItem([FromBody] CartSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustAcc) || string.IsNullOrWhiteSpace(request.ItemId))
            return BadRequest(new { success = false, message = "Customer and Item are required" });

        try
        {
            using var command = _context.Database.GetDbConnection().CreateCommand() as SqlCommand;
            command.CommandText = @"
                MERGE CartItems AS target
                USING (VALUES (@custAcc, @itemId, @quantity, @unitPrice)) 
                AS source (custAcc, itemId, quantity, unitPrice)
                ON target.custAcc = source.custAcc 
                   AND target.itemId COLLATE Arabic_CI_AI = source.itemId COLLATE Arabic_CI_AI
                WHEN MATCHED THEN
                    UPDATE SET 
                        quantity = source.quantity,
                        unitPrice = source.unitPrice,
                        updatedAt = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (custAcc, itemId, quantity, unitPrice)
                    VALUES (source.custAcc, source.itemId, source.quantity, source.unitPrice);";

            command.Parameters.Add(new SqlParameter("@custAcc", request.CustAcc));
            command.Parameters.Add(new SqlParameter("@itemId", request.ItemId));
            command.Parameters.Add(new SqlParameter("@quantity", request.Quantity));
            command.Parameters.Add(new SqlParameter("@unitPrice", request.UnitPrice));

            await _context.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            await _context.Database.CloseConnectionAsync();

            return Ok(new { success = true, message = "Cart updated successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Save Cart Error: " + ex.ToString());
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    // ====================== REMOVE ITEM ======================
    [HttpPost("remove")]
    public async Task<IActionResult> RemoveCartItem([FromBody] CartRemoveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustAcc) || string.IsNullOrWhiteSpace(request.ItemId))
            return BadRequest(new { success = false, message = "Customer and Item are required" });

        try
        {
            using var command = _context.Database.GetDbConnection().CreateCommand() as SqlCommand;
            command.CommandText = "DELETE FROM CartItems WHERE custAcc = @custAcc AND itemId = @itemId";

            command.Parameters.Add(new SqlParameter("@custAcc", request.CustAcc));
            command.Parameters.Add(new SqlParameter("@itemId", request.ItemId));

            await _context.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            await _context.Database.CloseConnectionAsync();

            return Ok(new { success = true, message = "Item removed from cart" });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Remove Cart Error: " + ex.ToString());
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    // ====================== CLEAR CART ======================
    [HttpPost("clear")]
    public async Task<IActionResult> ClearCart([FromBody] CartClearRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustAcc))
            return BadRequest(new { success = false, message = "Customer account is required" });

        try
        {
            using var command = _context.Database.GetDbConnection().CreateCommand() as SqlCommand;
            command.CommandText = "DELETE FROM CartItems WHERE custAcc = @custAcc";

            command.Parameters.Add(new SqlParameter("@custAcc", request.CustAcc));

            await _context.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            await _context.Database.CloseConnectionAsync();

            return Ok(new { success = true, message = "Cart cleared successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Clear Cart Error: " + ex.ToString());
            return StatusCode(500, new { success = false, message = ex.Message });
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

    private int GetInt(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }
        catch { return 0; }
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
}