using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seminar_1.Models;
using Seminar_1.Models.MetaData;

namespace Seminar_1.Controllers
{
    
    public class GioHangController : Controller
    {
        Entities db= new Entities();

        // Trả ra view chính cho trang giỏ hàng
        public ActionResult XemGiohang()
        {
            if(((ThanhVien)Session["DangNhap"])!=null)
            {
                int MaNguoiMua = ((ThanhVien)Session["DangNhap"]).MaTV;
                var lst = db.GioHangs.Where(n => n.MaNguoiMua == MaNguoiMua);
                return View(lst);
            }
            return RedirectToAction("DangNhap", "Home");
        }

        // Thêm một món hàng vào giỏ hàng, nếu đã có món đó thì tăng số lượng lên //Thêm mới bằng cách load lại trang để cập nhật icon cart
        public ActionResult ThemItemVaoGioHang(int MaSP, string strURL)
        {
            int MaKH = ((ThanhVien)Session["DangNhap"]).MaTV;
            SanPham sp = db.SanPhams.Find(MaSP);
            GioHang item = db.GioHangs.SingleOrDefault(n => n.MaNguoiMua == MaKH && n.MaSP == MaSP);
            if (item != null)
            {
                //nếu số lượng tồn còn thì đặt thành công
                if(sp.SoLuongTon>0)
                {
                    db.GioHangs.Remove(item);
                    db.SaveChanges();
                    item.SoLuong++;
                    item.DonGia += sp.Gia;
                    db.GioHangs.Add(item);
                    sp.SoLuongTon--;
                    db.SaveChanges();
                    return Redirect(strURL);
                }
                else
                {
                    return RedirectToAction("ThongBaoHetHang");
                }
            }else
            {
                //Khởi tạo item mới và thêm vào csdl giỏ hàng
                if (sp.SoLuongTon > 0)
                {
                    item = new GioHang();
                    item.MaNguoiMua = MaKH;
                    item.MaNguoiBan = sp.MaNguoiBan;
                    item.MaSP = sp.MaSP;
                    item.TenSP = sp.TenSP;
                    item.SoLuong = 1;
                    item.DonGia = sp.Gia;
                    item.HinhAnh = sp.HinhAnh;
                    sp.SoLuongTon--;
                    db.GioHangs.Add(item);
                    db.SaveChanges();
                    return Redirect(strURL);
                }
                else
                {
                    return RedirectToAction("ThongBaoHetHang");
                }
            }
        }
        //Trang thông báo hết hàng
        public ActionResult ThongBaoHetHang()
        {
            return View();
        }
        //Xóa item trong giỏ hàng
        public ActionResult XoaItemTrongGioHang(int MaSP,int MaKH, string strURL)
        {
            GioHang item = db.GioHangs.SingleOrDefault(n => n.MaNguoiMua == MaKH && n.MaSP == MaSP);
            db.GioHangs.Remove(item);
            db.SanPhams.Find(MaSP).SoLuongTon += item.SoLuong;

            db.SaveChanges();
            return Redirect(strURL);
        }
        //Chỉnh sửa số lượng trong giỏ hàng
        public ActionResult SuaSoLuongHang(int MaSP, int MaKH, bool sw, string strURL)
        {
            GioHang item = db.GioHangs.SingleOrDefault(n => n.MaNguoiMua == MaKH && n.MaSP == MaSP);
            SanPham sp = db.SanPhams.Find(MaSP);
            if(sw)
            {
                //Trường hợp muốn tăng số lượng
                if(sp.SoLuongTon>0)
                {
                    db.GioHangs.Remove(item);
                    db.SaveChanges();
                    item.SoLuong++;
                    item.DonGia += sp.Gia;
                    db.GioHangs.Add(item);
                    sp.SoLuongTon--;
                    db.SaveChanges();
                    return Redirect(strURL);
                }
                return RedirectToAction("ThongBaoHetHang");
            }
            //Trường hợp muốn giảm số lượng
            db.GioHangs.Remove(item);
            db.SaveChanges();
            if (item.SoLuong>1)
            {
                //nếu hàng trong giỏ lớn hơn 1
                item.SoLuong--;
                item.DonGia -= sp.Gia;
                db.GioHangs.Add(item);
                sp.SoLuongTon++;
                db.SaveChanges();
            }

            //nếu hàng trong giỏ ==1 thì refresh lại trang, db đã cập nhật ở dòng 106
            return Redirect(strURL);
        }
        //PartialView iconGioHang
        public ActionResult PartialViewIconGioHang()
        {
            int MaNguoiMua = ((ThanhVien)Session["DangNhap"]).MaTV;
            var lst = db.GioHangs.Where(n => n.MaNguoiMua == MaNguoiMua);
            ViewBag.SoLuongItem = lst.Count();
            return PartialView();
        }
        //
        public ActionResult DatHang()
        {
            ThanhVien tv = (ThanhVien)Session["dangnhap"];

            var grMNB = db.GioHangs.GroupBy(n => n.MaNguoiBan).OrderBy(n=>n.Key);
            var timeNow= DateTime.Now;
            while (grMNB.Count()>0)
            {
                Don ddhTemp = new Don();
                ddhTemp.MaNguoiMua = tv.MaTV;
                ddhTemp.MaNguoiBan = grMNB.First().Key;
                ddhTemp.TenNguoiMua = tv.TenTV;
                ddhTemp.TenNguoiBan = db.ThanhViens.SingleOrDefault(n => n.MaTV == ddhTemp.MaNguoiBan).TenTV;
                ddhTemp.NgayDat = timeNow;
                db.Dons.Add(ddhTemp);
                db.SaveChanges();
                foreach(var gh in grMNB.First())
                {
                    ChiTietDon ctdh = new ChiTietDon();
                    ctdh.MaD = ddhTemp.MaD;
                    ctdh.MaSP = gh.MaSP;
                    var tenSP = db.SanPhams.SingleOrDefault(n => n.MaSP == ctdh.MaSP).TenSP;
                    ctdh.TenSP = tenSP;
                    ctdh.SoLuong = gh.SoLuong;
                    ctdh.DonGia = gh.DonGia;
                    db.ChiTietDons.Add(ctdh);
                    db.GioHangs.Remove(gh);
                }
                db.SaveChanges();
                //grMNB = grMNB.Skip(1).OrderBy(n => n.Key);
            }
            
            return RedirectToAction("Index", "Home");
        }
        //
        public ActionResult DonHangCuaBan()
        {
            return View();
        }
        //
        public ActionResult DonChuaXacNhanPartial()
        {
            ThanhVien tv = (ThanhVien)Session["dangnhap"];
            var lst = db.Dons.Where(n => n.MaNguoiMua == tv.MaTV && n.TrangThai==(int)TrangThaiDonHang.MoiTao);
            ViewBag.IndexPage = 1;
            return PartialView(lst);
        }
        public ActionResult DonDaXacNhanPartial()
        {
            ThanhVien tv = (ThanhVien)Session["dangnhap"];
            var lst = db.Dons.Where(n => n.MaNguoiMua == tv.MaTV && n.TrangThai == (int)TrangThaiDonHang.DangGuiDenDiemTapKet);
            ViewBag.IndexPage = 2;
            return PartialView(lst);
        }
        public ActionResult DonDangGiaoPartial()
        {
            ThanhVien tv = (ThanhVien)Session["dangnhap"];
            var lst = db.Dons.Where(n => n.MaNguoiMua == tv.MaTV && n.TrangThai == (int)TrangThaiDonHang.DaDuyet);
            ViewBag.IndexPage = 3;
            return PartialView(lst);
        }
        public ActionResult DonDaNhanPartial()
        {
            ThanhVien tv = (ThanhVien)Session["dangnhap"];
            var lst = db.Dons.Where(n => n.MaNguoiMua == tv.MaTV && n.TrangThai == (int)TrangThaiDonHang.DaGiao);
            ViewBag.IndexPage = 4;
            return PartialView(lst);
        }
        
        ///
        public ActionResult ChiTietDonHang(int MaDH, int indexPage)
        {
            ViewBag.IndexPage = indexPage;
            var lst = db.ChiTietDons.Where(n => n.MaD == MaDH);
            return PartialView(lst);
        }
        public ActionResult HuyDonHang(int MaDH)
        {
            var dTmp = db.Dons.SingleOrDefault(n => n.MaD == MaDH);
            db.Dons.Remove(dTmp);
            db.SaveChanges();
            return RedirectToAction("DonChuaXacNhanPartial");
        }
        public ActionResult XacNhanNhanHang(int MaDH)
        {
            var dhTmp = db.Dons.SingleOrDefault(n => n.MaD == MaDH);
            dhTmp.TrangThai = (int)TrangThaiDonHang.DaGiao;
            dhTmp.NgayGiao = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("DonDangGiaoPartial");
        }
    }
}