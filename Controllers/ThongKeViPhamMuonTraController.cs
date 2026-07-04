using System.Data;
using CSDLNC.Data;
using CSDLNC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CSDLNC.Controllers;

[Authorize]
public class ThongKeViPhamMuonTraController : Controller
{
    private readonly ThuVienDbContext _db;

    public ThongKeViPhamMuonTraController(ThuVienDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(ThongKeViPhamMuonTraViewModel model)
    {
        if (!HasPermission("Q009"))
            return RedirectToAction("AccessDenied", "Account");

        if (model.Thang == 0)
            model.Thang = DateTime.Today.Month;

        if (model.Nam == 0)
            model.Nam = DateTime.Today.Year;

        if (string.IsNullOrWhiteSpace(model.LoaiViPham))
            model.LoaiViPham = "Tất cả";

        if (ModelState.IsValid)
            model.KetQua = await LayThongKeAsync(model);

        return View(model);
    }

    private async Task<List<ThongKeViPhamMuonTraItemViewModel>> LayThongKeAsync(ThongKeViPhamMuonTraViewModel model)
    {
        var results = new List<ThongKeViPhamMuonTraItemViewModel>();
        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_ThongKeViPhamMuonTraTheoThang";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Thang", SqlDbType.Int) { Value = model.Thang });
            command.Parameters.Add(new SqlParameter("@Nam", SqlDbType.Int) { Value = model.Nam });
            command.Parameters.Add(new SqlParameter("@LoaiViPham", SqlDbType.NVarChar, 50)
            {
                Value = string.IsNullOrWhiteSpace(model.LoaiViPham) ? DBNull.Value : model.LoaiViPham
            });

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(new ThongKeViPhamMuonTraItemViewModel
                {
                    MaSinhVien = ReadString(reader, "MaSinhVien"),
                    HoTen = ReadString(reader, "HoTen"),
                    Lop = ReadNullableString(reader, "Lop"),
                    Khoa = ReadNullableString(reader, "Khoa"),
                    LoaiViPham = ReadString(reader, "LoaiViPham"),
                    MucDoViPham = ReadString(reader, "MucDoViPham"),
                    HinhThucXuLy = ReadString(reader, "HinhThucXuLy"),
                    NgayLap = Convert.ToDateTime(reader["NgayLap"]),
                    GhiChu = ReadString(reader, "GhiChu")
                });
            }
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }

        return results;
    }

    private static string ReadString(IDataRecord reader, string name)
    {
        return reader[name]?.ToString() ?? "";
    }

    private static string? ReadNullableString(IDataRecord reader, string name)
    {
        return reader[name] == DBNull.Value ? null : reader[name].ToString();
    }

    private bool HasPermission(string permission)
    {
        return User.HasClaim("Permission", permission)
            || User.HasClaim("Permission", "Q001")
            || User.HasClaim("Permission", "Q010");
    }
}
