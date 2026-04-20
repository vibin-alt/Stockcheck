using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context)
    {
        _context = context;
    }

    // ====================== SEARCH / POPULAR PRODUCTS ======================
    [HttpGet("search")]
    public async Task<IActionResult> SearchStock(
        [FromQuery] string ser = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Allow empty search for Popular Products
        bool isPopularRequest = string.IsNullOrWhiteSpace(ser);

        if (!isPopularRequest && ser.Trim().Length < 1)
        {
            return BadRequest(new { success = false, message = "Search term is required" });
        }

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100; // Limit maximum items per page

        try
        {
            var items = new List<ItemDto>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "EXEC Sp_PortalStock @ser";
            command.CommandType = CommandType.Text;
            command.Parameters.Add(new SqlParameter("@ser", isPopularRequest ? "" : ser.Trim()));

            await _context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();

            int rowNumber = 0;
            int totalRecords = 0;

            while (await reader.ReadAsync())
            {
                totalRecords++;
                rowNumber++;

                if (rowNumber >= (page - 1) * pageSize + 1 && rowNumber <= page * pageSize)
                {
                    var item = new ItemDto
                    {
                        Id = GetString(reader, "partNumber"),
                        Oem = GetString(reader, "oem"),
                        PartNumber = GetString(reader, "partNumber"),
                        Description = GetString(reader, "description"),
                        Specification = GetString(reader, "specification"),
                        Brand = GetString(reader, "Brand"),
                        Model = GetString(reader, "Model"),
                        StockQty = GetDecimal(reader, "stockQty"),
                        Price = GetDecimal(reader, "price"),
                        Category = GetString(reader, "category"),
                        Picture = GetPictureAsBase64(reader, "picture"),
                        Substitute = GetString(reader, "Substitutes")
                    };

                    items.Add(item);
                }
            }

            await _context.Database.CloseConnectionAsync();

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return Ok(new
            {
                success = true,
                message = isPopularRequest ? "Popular products loaded successfully" : $"Found {totalRecords} item(s)",
                data = items,
                pagination = new
                {
                    totalRecords,
                    totalPages,
                    currentPage = page,
                    pageSize,
                    hasNextPage = page < totalPages,
                    hasPreviousPage = page > 1
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Search Error: " + ex.ToString());
            return StatusCode(500, new
            {
                success = false,
                message = "Internal server error",
                error = ex.Message
            });
        }
    }

    // ====================== GET CATEGORIES ======================
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = new List<CategoryDto>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "EXEC Sp_PortalCategories";
            command.CommandType = CommandType.Text;

            await _context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var cat = new CategoryDto
                {
                    Name = GetString(reader, "category"),
                    ItemCount = GetInt(reader, "itemCount")
                };
                categories.Add(cat);
            }

            await _context.Database.CloseConnectionAsync();

            return Ok(new
            {
                success = true,
                message = "Categories retrieved successfully",
                data = categories
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Internal server error",
                error = ex.Message
            });
        }
    }

    // ====================== HELPER METHODS ======================
    private string GetString(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }
        catch
        {
            return string.Empty;
        }
    }

    private decimal GetDecimal(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }
        catch
        {
            return 0;
        }
    }

    private int GetInt(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }
        catch
        {
            return 0;
        }
    }

    private string GetPictureAsBase64(DbDataReader reader, string columnName)
    {
        try
        {
            int ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
                return string.Empty;

            byte[]? bytes = reader[columnName] as byte[];
            if (bytes != null && bytes.Length > 0)
            {
                return $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}