using System.Data;
using System.Globalization;
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
        {
            model.KetQua = await LayThongKeAsync(model);
            PopulateDashboard(model);
        }

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

    private static void PopulateDashboard(ThongKeViPhamMuonTraViewModel model)
    {
        model.TongViPham = model.KetQua.Count;
        model.ViPhamQuaHan = model.KetQua.Count(x => IsQuaHan(x.LoaiViPham));
        model.ViPhamHongMat = model.KetQua.Count(x => IsHongMat(x.LoaiViPham));
        if (model.TongViPham > 0 && model.ViPhamHongMat == 0)
            model.ViPhamHongMat = model.TongViPham - model.ViPhamQuaHan;
        model.SoDocGiaViPham = model.KetQua.Select(x => x.MaSinhVien).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        model.TongTienPhat = model.KetQua.Sum(x => ExtractAmount(x.HinhThucXuLy));

        model.PhanBoLoaiViPham = new List<ThongKeChartItemViewModel>
        {
            new() { Label = "Quá hạn", Value = model.ViPhamQuaHan },
            new() { Label = "Hỏng/Mất", Value = model.ViPhamHongMat }
        };

        model.TopDocGiaViPham = model.KetQua
            .GroupBy(x => new { x.MaSinhVien, x.HoTen })
            .Select(x => new ThongKeChartItemViewModel
            {
                Label = $"{x.Key.MaSinhVien} - {x.Key.HoTen}",
                Value = x.Count(),
                Amount = x.Sum(item => ExtractAmount(item.HinhThucXuLy))
            })
            .OrderByDescending(x => x.Value)
            .ThenByDescending(x => x.Amount)
            .Take(5)
            .ToList();
    }

    private static bool IsQuaHan(string value)
    {
        return value.Contains("quá hạn", StringComparison.OrdinalIgnoreCase)
            || value.Contains("qua han", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHongMat(string value)
    {
        return value.Contains("hỏng", StringComparison.OrdinalIgnoreCase)
            || value.Contains("mất", StringComparison.OrdinalIgnoreCase)
            || value.Contains("hong", StringComparison.OrdinalIgnoreCase)
            || value.Contains("mat", StringComparison.OrdinalIgnoreCase);
    }

    private static decimal ExtractAmount(string value)
    {
        var amountText = new string(value.Where(x => char.IsDigit(x) || x == '.' || x == ',').ToArray());

        if (string.IsNullOrWhiteSpace(amountText))
            return 0;

        amountText = amountText.Replace(",", ".");
        return decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)
            ? amount
            : 0;
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
