using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Seminar_1.Models;
using Seminar_1.Models.MetaData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seminar_1.Controllers
{
    public class AdminController : Controller
    {
        Entities db = new Entities();
        public ActionResult Index()
        {
            if(Session["dangnhap"]==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            if(((ThanhVien)Session["dangnhap"]).MaTV==3)
            {
                return View();
            }
            else
            {
                Response.StatusCode = 404;
                return null;
            }
        }
        public ActionResult DonHangChuaDuyet()
        {
            if (Session["DangNhap"] == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var lst = db.Dons.Where(n=>n.TrangThai==(int)TrangThaiDonHang.DangGuiDenDiemTapKet);
            return PartialView(lst);
        }
        public ActionResult DonHangDaDuyet()
        {
            if (Session["DangNhap"] == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var lst = db.Dons.Where(n => n.TrangThai == (int)TrangThaiDonHang.DaDuyet);
            return PartialView(lst);
        }
        
        public ActionResult DonHangDaGiao()
        {
            if (Session["DangNhap"] == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var lst = db.Dons.Where(n => n.TrangThai == (int)TrangThaiDonHang.DaGiao);
            return PartialView(lst);
        }
        //Xuất Excel
        private Stream CreateExcelFile(IEnumerable<ChiTietDon> lstCTDH,Don dh,Stream stream = null)
        {
            var list = lstCTDH;

            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                // Tạo author cho file Excel
                excelPackage.Workbook.Properties.Author = "Hanker";
                // Tạo title cho file Excel
                excelPackage.Workbook.Properties.Title = "EPP test background";
                // thêm tí comments vào làm màu 
                excelPackage.Workbook.Properties.Comments = "This is my fucking generated Comments";
                // Add Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("First Sheet");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var workSheet = excelPackage.Workbook.Worksheets[1];
                // Đỗ data vào Excel file
                //workSheet.Cells[1, 1].LoadFromCollection(list, true, TableStyles.Dark9);
                BindingFormatForExcel(workSheet, list, dh);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
        private void BindingFormatForExcel(ExcelWorksheet worksheet, IEnumerable<ChiTietDon> listItems, Don dh)
        {
            // Set default width cho tất cả column
            worksheet.DefaultColWidth = 25;
            // Tự động xuống hàng khi text quá dài
            worksheet.Cells.Style.WrapText = true;
            // Tạo header
            int numRowInfo = 8;
            worksheet.Cells[numRowInfo, 1].Value = "Mã CTDH";
            worksheet.Cells[numRowInfo, 2].Value = "Tên sản phẩm";
            worksheet.Cells[numRowInfo, 3].Value = "Số lượng";
            worksheet.Cells[numRowInfo, 4].Value = "Thành tiền(vnd)";

            // Lấy range vào tạo format cho range đó ở đây là từ A2 tới E2
            using (var range = worksheet.Cells["A8:D8"])
            {
                // Set PatternType
                range.Style.Fill.PatternType = ExcelFillStyle.DarkGray;
                // Set Màu cho Background
                range.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
                // Canh giữa cho các text
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                // Set Font cho text  trong Range hiện tại
                range.Style.Font.SetFromFont(new Font("Arial", 10));
                // Set Border
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                // Set màu ch Border
                range.Style.Border.Bottom.Color.SetColor(Color.Blue);
            }

            // Đỗ dữ liệu từ list vào 
            int i = 0;
            decimal tongTien = 0;
            foreach (ChiTietDon item in listItems)
            {
                worksheet.Cells[i + numRowInfo+1, 1].Value = item.MaCTD;
                var tenSP = db.SanPhams.SingleOrDefault(n => n.MaSP == item.MaSP).TenSP;
                worksheet.Cells[i + numRowInfo+1, 2].Value = tenSP;
                worksheet.Cells[i + numRowInfo+1, 3].Value = item.SoLuong;
                worksheet.Cells[i + numRowInfo+1, 4].Value = item.DonGia;
                // Format lại color nếu như thỏa điều kiện
                //if (item.Gia > 20050)
                //{
                //    // Ở đây chúng ta sẽ format lại theo dạng fromRow,fromCol,toRow,toCol
                //    using (var range = worksheet.Cells[i + numRowInfo+1, 1, i + numRowInfo+1, 5])
                //    {
                //        // Format text đỏ và đậm
                //        range.Style.Font.Color.SetColor(Color.Red);
                //        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //        range.Style.Font.Bold = true;
                //    }
                //}
                tongTien += item.DonGia;
                i++;
            }
            db.ThanhViens.SingleOrDefault(n => n.MaTV == dh.MaNguoiBan).Money += tongTien;
            // Format lại định dạng xuất ra ở cột Money
            worksheet.Cells[numRowInfo+1, 4, listItems.Count() + numRowInfo, 4].Style.Numberformat.Format = "#,##";
            // fix lại width của column với minimum width là 15
            worksheet.Cells[1, 1, listItems.Count() + numRowInfo, 4].AutoFitColumns(25);

            //// Thực hiện tính theo formula trong excel
            //// Hàm Sum 

            //worksheet.Cells[listItems.Count() + 3, 4].Formula = "SUM(D2:D" + (listItems.Count() + 1) + ")";
            //// Hàm SumIf
            worksheet.Cells["A1:B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            // Set Font cho text  trong Range hiện tại
            worksheet.Cells["A1:A7"].Style.Font.SetFromFont(new Font("Arial", 10));
            worksheet.Cells["A1:A7"].Style.Font.Bold = true;
            worksheet.Cells[1,1].Value = "FShop";
            worksheet.Cells[2, 1].Value = "Người mua:";
            worksheet.Cells[3, 1].Value = "Ngày đặt:";
            worksheet.Cells[4, 1].Value = "Địa chỉ:";
            worksheet.Cells[5, 1].Value = "Tổng tiền:";
            worksheet.Cells[6, 1].Value = "Sđt:";
            worksheet.Cells[7, 1].Value = "Người bán:";

            var tenNguoiMua = db.ThanhViens.SingleOrDefault(n => n.MaTV == dh.MaNguoiMua).TenTV;
            var tenNguoiBan = db.ThanhViens.SingleOrDefault(n => n.MaTV == dh.MaNguoiBan).TenTV;
            var diaChi = db.ThanhViens.SingleOrDefault(n => n.MaTV == dh.MaNguoiMua).DiaChi;
            var sdt = db.ThanhViens.SingleOrDefault(n => n.MaTV == dh.MaNguoiMua).SDT;
            worksheet.Cells[2, 2].Value = tenNguoiMua;
            worksheet.Cells[3, 2].Value = dh.NgayDat.ToString();
            worksheet.Cells[4, 2].Value = diaChi;
            worksheet.Cells[5, 2].Formula = "SUM(D9:D"+(listItems.Count()+8)+")";
            worksheet.Cells[5, 2].Style.Numberformat.Format = "#,##";
            worksheet.Cells[6, 2].Value = sdt.ToString();
            worksheet.Cells[7, 2].Value = tenNguoiBan;
            //worksheet.Cells[listItems.Count() + 4, 4].Formula = "SUMIF(D2:D" + (listItems.Count() + 1) + ",\">20050\")";
            //// Tinh theo %
            //worksheet.Cells[listItems.Count() + 5, 3].Value = "Percentatge: ";
            //worksheet.Cells[listItems.Count() + 5, 4].Style.Numberformat.Format = "0.00%";
            //// Dòng này có nghĩa là ở column hiện tại lấy với địa chỉ (Row hiện tại - 1)/ (Row hiện tại - 2) Cùng một colum
            //worksheet.Cells[listItems.Count() + 5, 4].FormulaR1C1 = "(R[-1]C/R[-2]C)";
        }
        public ActionResult DuyetDonHang(int MaDH)
        {
            var lstCTDH = db.ChiTietDons.Where(n => n.MaD == MaDH);
            var dh = db.Dons.SingleOrDefault(n => n.MaD == MaDH);
            var stream = CreateExcelFile(lstCTDH, dh);
            // Tạo buffer memory strean để hứng file excel
            var buffer = stream as MemoryStream;
            // Đây là content Type dành cho file excel, còn rất nhiều content-type khác nhưng cái này mình thấy okay nhất
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
            // File name của Excel này là ExcelDemo
            var fileName = "FShop_" + dh.MaD + "_" + dh.NgayDat.ToString() + ".xlsx";
            Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
            // Lưu file excel của chúng ta như 1 mảng byte để trả về response
            Response.BinaryWrite(buffer.ToArray());
            // Send tất cả ouput bytes về phía clients
            Response.Flush();
            Response.End();
            dh.TrangThai = (int)TrangThaiDonHang.DaDuyet;
            db.SaveChanges();

            return Content("<script>window.location.reload();</script>");
        }
    }
}