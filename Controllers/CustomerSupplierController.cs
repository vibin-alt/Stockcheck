//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using StkPortal.Models;

//namespace StkPortal.Controllers
//{
//    public class CustomerSupplierController
//    {
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StkPortal.Models;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class CustomerSupplierController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomerSupplierController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("insert-update")]
    public async Task<IActionResult> InsertOrUpdateCustomerSupplier([FromBody] CustomerSupplierDto dto)
    {
        try
        {
            var parameters = new[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@mod", dto.Mod),
                new Microsoft.Data.SqlClient.SqlParameter("@cscode", dto.Cscode),
                new Microsoft.Data.SqlClient.SqlParameter("@ac_code", dto.AcCode),
                new Microsoft.Data.SqlClient.SqlParameter("@type", dto.Type),
                new Microsoft.Data.SqlClient.SqlParameter("@cs_des", dto.CsDes),
                new Microsoft.Data.SqlClient.SqlParameter("@balance_open", dto.BalanceOpen),
                new Microsoft.Data.SqlClient.SqlParameter("@dr", dto.Dr),
                new Microsoft.Data.SqlClient.SqlParameter("@cr", dto.Cr),
                new Microsoft.Data.SqlClient.SqlParameter("@fc", dto.Fc),
                new Microsoft.Data.SqlClient.SqlParameter("@fdr", dto.Fdr),
                new Microsoft.Data.SqlClient.SqlParameter("@fcr", dto.Fcr),
                new Microsoft.Data.SqlClient.SqlParameter("@cmpcode", dto.Cmpcode),
                new Microsoft.Data.SqlClient.SqlParameter("@divcode", dto.Divcode),
                new Microsoft.Data.SqlClient.SqlParameter("@keywords", dto.Keywords),
                new Microsoft.Data.SqlClient.SqlParameter("@adr1", dto.Adr1),
                new Microsoft.Data.SqlClient.SqlParameter("@adr2", dto.Adr2),
                new Microsoft.Data.SqlClient.SqlParameter("@adr3", dto.Adr3),
                new Microsoft.Data.SqlClient.SqlParameter("@adr4", dto.Adr4),
                new Microsoft.Data.SqlClient.SqlParameter("@adr5", dto.Adr5),
                new Microsoft.Data.SqlClient.SqlParameter("@tel", dto.Tel),
                new Microsoft.Data.SqlClient.SqlParameter("@fax", dto.Fax),
                new Microsoft.Data.SqlClient.SqlParameter("@mobile", dto.Mobile),
                new Microsoft.Data.SqlClient.SqlParameter("@salesman", dto.Salesman),
                new Microsoft.Data.SqlClient.SqlParameter("@areacode", dto.Areacode),
                new Microsoft.Data.SqlClient.SqlParameter("@email", dto.Email),
                new Microsoft.Data.SqlClient.SqlParameter("@remarks", dto.Remarks),
                new Microsoft.Data.SqlClient.SqlParameter("@country", dto.Country),
                new Microsoft.Data.SqlClient.SqlParameter("@state", dto.State),
                new Microsoft.Data.SqlClient.SqlParameter("@crlimit", dto.Crlimit),
                new Microsoft.Data.SqlClient.SqlParameter("@duedays", dto.Duedays),
                new Microsoft.Data.SqlClient.SqlParameter("@terms", dto.Terms),
                new Microsoft.Data.SqlClient.SqlParameter("@crmethod", dto.Crmethod),
                new Microsoft.Data.SqlClient.SqlParameter("@cntrlcode", dto.Cntrlcode),
                new Microsoft.Data.SqlClient.SqlParameter("@avtive", dto.Avtive),
                new Microsoft.Data.SqlClient.SqlParameter("@colourcode", dto.Colourcode),
                new Microsoft.Data.SqlClient.SqlParameter("@price_cat", dto.PriceCat),
                new Microsoft.Data.SqlClient.SqlParameter("@grade", dto.Grade),
                new Microsoft.Data.SqlClient.SqlParameter("@vatregno", dto.Vatregno),
                new Microsoft.Data.SqlClient.SqlParameter("@shippingadr", dto.Shippingadr),
                new Microsoft.Data.SqlClient.SqlParameter("@create_user", dto.CreateUser),
                new Microsoft.Data.SqlClient.SqlParameter("@discount", dto.Discount),
                new Microsoft.Data.SqlClient.SqlParameter("@margin", dto.Margin),
                new Microsoft.Data.SqlClient.SqlParameter("@doduedays", dto.Doduedays),
                new Microsoft.Data.SqlClient.SqlParameter("@user_name", dto.UserName),
                new Microsoft.Data.SqlClient.SqlParameter("@pass_word", dto.PassWord)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[sp_Insertcustomersupplier_Online] @mod, @cscode, @ac_code, @type, @cs_des, @balance_open, @dr, @cr, @fc, @fdr, @fcr, @cmpcode, @divcode, @keywords, @adr1, @adr2, @adr3, @adr4, @adr5, @tel, @fax, @mobile, @salesman, @areacode, @email, @remarks, @country, @state, @crlimit, @duedays, @terms, @crmethod, @cntrlcode, @avtive, @colourcode, @price_cat, @grade, @vatregno, @shippingadr, @create_user, @discount, @margin, @doduedays, @user_name, @pass_word",
                parameters);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = dto.Mod == "I" ? "Customer/Supplier inserted successfully" : "Customer/Supplier updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            });
        }
    }
}