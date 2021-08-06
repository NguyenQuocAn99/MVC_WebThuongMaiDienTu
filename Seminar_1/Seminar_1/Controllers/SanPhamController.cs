using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seminar_1.Models;
using PagedList;

namespace Seminar_1.Controllers
{

    public class SanPhamController : Controller
    {
        Entities db = new Entities();
        // Trả về trang chi tiết SanPham
        public ActionResult ChiTietSanPham(int MaSP, int MaLoaiSP, int MaNguoiBan) 
        {
            SanPham sanpham = db.SanPhams.Find(MaSP);
            ViewBag.LoaiSP = db.LoaiSPs.Find(sanpham.MaLoaiSP).TenLSP;
            ViewBag.LoaiDM = db.DanhMucSPs.Find(db.LoaiSPs.Find(sanpham.MaLoaiSP).MaDM).TenDM;
            ViewBag.TenNguoiBan = db.ThanhViens.Find(sanpham.MaNguoiBan).TenTV;
            ViewBag.SP = sanpham;
            ViewBag.SDT = db.ThanhViens.Find(sanpham.MaNguoiBan).SDT;
            ViewBag.DiaChi = db.ThanhViens.Find(sanpham.MaNguoiBan).DiaChi;
            return View();
        }
        //Trả ra trang các sản phẩm được lọc với các tham số truyền vào gồm mã danh mục, mã loại sản phẩm
        public ActionResult SanPham(int MaDM, int MaLSP, int? page)
        {
            var loaispData = db.LoaiSPs.Where(n => n.MaDM == MaDM);
       
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var sanPham = GetSanPham(MaDM, MaLSP);
            ViewBag.sanpham = sanPham.OrderBy(n => n.TenSP).ToPagedList(pageNumber, pageSize);
            ViewBag.loaispData = loaispData;
            ViewBag.MaLSP = MaLSP;
            return View();
        }
        //Tìm kiếm sử dụng Ajax
        public ActionResult TimKiemSanPhamPartial(string keySearch, int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            int? MaDM = (TempData["MaDM"] as int?);
            int? MaLSP = (TempData["MaLSP"] as int?);
            TempData.Keep();
            IEnumerable<SanPham> sp = GetSanPham(MaDM??-1, MaLSP??-1);
            
            sp = sp.Where(n => n.TenSP.ToLower().Contains(keySearch.ToLower()));
            ViewBag.keySearch = keySearch;
            return PartialView(sp.OrderBy(n=>n.TenSP).ToPagedList(pageNumber,pageSize)) ;
        }
        // Lấy list sản phẩm theo DM và LSP
        public IEnumerable<SanPham> GetSanPham(int MaDM, int MaLSP)
        {
            var loaispData = db.LoaiSPs.Where(n => n.MaDM == MaDM);
            if (MaLSP == 0)
            {
                //Trường hợp chọn tất cả sản phẩm thuộc danh mục, vì mã danh mục không nằm trong sp nên phải thực hiện inner join
                //var lsp = db.LoaiSPs.Where(n => n.MaDM == MaDM);

                var sanpham = from loai in loaispData
                              join sp in db.SanPhams
                              on loai.MaLSP equals sp.MaLoaiSP
                              select sp;
               
                return sanpham;
            }
            else
            {
                //Trường hợp không quan tâm đến danh mục vì có thể định danh bằng loại
                var sanpham = db.SanPhams.Where(n => n.MaLoaiSP == MaLSP);
                return sanpham;
            }

        }
        public ActionResult ShowSanPhamShop(int MaNguoiBan)
        {
            var sanPham = db.SanPhams.Where(n => n.MaNguoiBan == MaNguoiBan);
            ViewBag.TenNguoiBan = db.ThanhViens.Find(MaNguoiBan).TenTV;
            return View(sanPham);
        }

    }
}