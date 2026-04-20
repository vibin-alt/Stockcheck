using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto?.UserName) || string.IsNullOrWhiteSpace(dto?.PassWord))
            {
                return BadRequest(new { success = false, message = "Username and Password are required" });
            }

            var parameters = new[]
            {
                new SqlParameter("@user_name", SqlDbType.VarChar) { Value = dto.UserName },
                new SqlParameter("@pass_word", SqlDbType.VarChar) { Value = dto.PassWord }
            };

            // Using raw ADO.NET - Most reliable for stored procedures
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "EXEC [dbo].[UserPortalLogin] @user_name, @pass_word";
            command.CommandType = CommandType.Text;
            command.Parameters.AddRange(parameters);

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();

            LoginResponse? result = null;

            if (await reader.ReadAsync())
            {
                result = new LoginResponse
                {
                    Success = reader.GetBoolean(reader.GetOrdinal("Success")),
                    Message = reader.GetString(reader.GetOrdinal("Message")),
                    Data = reader.IsDBNull(reader.GetOrdinal("Data"))
                           ? null
                           : reader.GetString(reader.GetOrdinal("Data"))
                };
            }

            await _context.Database.CloseConnectionAsync();

            if (result == null)
            {
                return BadRequest(new { success = false, message = "No response from database" });
            }

            if (result.Success)
            {
                UserData? userData = null;
                if (!string.IsNullOrEmpty(result.Data))
                {
                    try
                    {
                        userData = JsonSerializer.Deserialize<UserData>(result.Data);
                    }
                    catch (Exception jsonEx)
                    {
                        Console.WriteLine("JSON Error: " + jsonEx.Message);
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    user = userData
                });
            }
            else
            {
                return Unauthorized(new
                {
                    success = false,
                    message = result.Message ?? "Invalid username or password"
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Login Error: " + ex.ToString());

            return StatusCode(500, new
            {
                success = false,
                message = "Internal server error",
                error = ex.Message
            });
        }
    }
}