using Seminar_1.Models;
using Seminar_1.Models.MetaData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seminar_1.Controllers
{
    public class DashBoardController : Controller
    {
        Entities db = new Entities();
        // GET: DashBoard
        public ActionResult Index()
        {
            return View();
        }
        ///Trả về partial view tạo mới sản phẩm
        [HttpGet]
        public ActionResult TaoMoiSanPham()
        {
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSPs.OrderBy(n => n.TenLSP), "MaLSP", "TenLSP");
            return PartialView();
        }
        [HttpPost]
        public ActionResult TaoMoiSanPham(SanPham sp, HttpPostedFileBase HinhAnh)
        {

            if (HinhAnh.ContentLength > 0)
            {
                var fileName = Path.GetFileName(HinhAnh.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                if (!System.IO.File.Exists(path))
                {
                    HinhAnh.SaveAs(path);
                }
                var path2 = "/Content/images/"+ fileName;
                sp.HinhAnh = path2;
                sp.NgayDang = DateTime.Now;
                sp.MaNguoiBan = ((ThanhVien)Session["dangnhap"]).MaTV;
                db.SanPhams.Add(sp);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        ///Trả về partialview Sản phẩm của bạn
        [HttpGet]
        public ActionResult SanPhamCuaBan()
        {
            if (Session["DangNhap"] == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int MaNguoiBan = ((ThanhVien)Session["DangNhap"]).MaTV;
            var lst = db.SanPhams.Where(n => n.MaNguoiBan == MaNguoiBan);
            //List<GioHang> lst = LayDataGioHang();
            return PartialView(lst);
        }
        ///Trả về partialview Chinh sửa sản phẩm
        [HttpGet]
        public ActionResult ChinhSuaSanPham(int? MaSP)
        {
            if (Session["DangNhap"] == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return PartialView(sp);
        }
        [HttpPost]
        public ActionResult ChinhSuaSanPham(SanPham model, HttpPostedFileBase HinhAnh)
        {
            if (HinhAnh != null)
            {
                var fileName = Path.GetFileName(HinhAnh.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                if (!System.IO.File.Exists(path))
                {
                    HinhAnh.SaveAs(path);
                }
                var path2 = "/Content/images/" + fileName;
                model.HinhAnh = path2;
            }
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("SanPhamCuaBan");
        }
        ///
        [HttpGet]
        public ActionResult XoaSanPham(int? MaSP)
        {
            int MaNguoiBan = ((ThanhVien)Session["DangNhap"]).MaTV;
            var sp = db.SanPhams.SingleOrDefault(n => n.MaNguoiBan == MaNguoiBan && n.MaSP == MaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SanPhams.Remove(sp);
            db.SaveChanges();
            return RedirectToAction("SanPhamCuaBan");
        }
        //
        public ActionResult DonHangChuaGui()
        {

            if (Session["DangNhap"] == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int MaNguoiBan = ((ThanhVien)Session["DangNhap"]).MaTV;
            var lst = db.Dons.Where(n => n.MaNguoiBan == MaNguoiBan && n.TrangThai == (int)TrangThaiDonHang.MoiTao);
            return PartialView(lst);
        }
        public ActionResult GuiHang(int MaDH)
        {
            var dh = db.Dons.SingleOrDefault(n => n.MaD == MaDH);
            dh.TrangThai = (int)TrangThaiDonHang.DangGuiDenDiemTapKet;
            db.SaveChanges();
            var lst = db.Dons.Where(n => n.MaNguoiBan == dh.MaNguoiBan && n.TrangThai == (int)TrangThaiDonHang.MoiTao);
            return PartialView(lst);
        }
        public ActionResult ChiTietDonHang(int MaDH)
        {
            var lst = db.ChiTietDons.Where(n => n.MaD == MaDH);
            return PartialView(lst);
        }
        public ActionResult SanpPhamDaBan()
        {
            if (Session["DangNhap"] == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int MaNguoiBan = ((ThanhVien)Session["DangNhap"]).MaTV;
            var lstD = db.Dons.Where(n => n.MaNguoiBan == MaNguoiBan && n.TrangThai==(int)TrangThaiDonHang.DaDuyet || n.TrangThai == (int)TrangThaiDonHang.DaGiao).ToList();
            var lsCTD = new List<ChiTietDon>();
            foreach(var dh in lstD)
            {
                var smallLstCTD = db.ChiTietDons.Where(n => n.MaD == dh.MaD).ToList();
                foreach(var ctd in smallLstCTD)
                {
                    lsCTD.Add(ctd);
                }
            }
            var grCTD = lsCTD.GroupBy(n => n.MaSP);
            var lstSPDaBan = new List<SanPham>();
            var tongDoanhThu = (decimal)0;
            foreach(var gr in grCTD)
            {
                SanPham spTemp = new SanPham();
                
                foreach(var ctd in gr)
                {
                    if(ctd==gr.First())
                    {
                        spTemp.MaSP = gr.Key;
                        spTemp.TenSP = ctd.TenSP;
                        spTemp.Gia = ctd.DonGia;
                        spTemp.SoLuongTon = ctd.SoLuong;
                    }
                    else
                    {
                        spTemp.SoLuongTon += ctd.SoLuong;
                        spTemp.Gia += ctd.DonGia;
                    }
                    
                }
                lstSPDaBan.Add(spTemp);
                tongDoanhThu += spTemp.Gia;

            }
            ViewBag.TongDoanhThu = tongDoanhThu;
            return PartialView(lstSPDaBan);
        }
    }
}