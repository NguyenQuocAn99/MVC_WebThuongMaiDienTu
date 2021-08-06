using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seminar_1.Models;
using System.Security.Cryptography;
using System.Text;

namespace Seminar_1.Controllers
{
    public class HomeController : Controller
    {
        Entities db = new Entities();
        public ActionResult Index()
        {
            var listDanhMuc = db.DanhMucSPs;
            var listSanPham = db.SanPhams.OrderByDescending(n => n.MaSP);
            ViewBag.LDM = listDanhMuc;
            ViewBag.LSP = listSanPham;
            return View();
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(ThanhVien tv, FormCollection f)
        {
            if (ModelState.IsValid)
            {

                if (tv.PassWord== f["confirmMK"])
                {
                    var passMD5 = HashMD5(tv.PassWord);
                    tv.PassWord = passMD5;
                    if (db.ThanhViens.SingleOrDefault(n => n.UserName== tv.UserName) == null)
                    {

                        db.ThanhViens.Add(tv);
                        db.SaveChanges();
                        return Content("<script>window.location.reload();</script>");
                    }
                    else
                    {
                        return Content("Tài khoản đã tồn tại!");
                    }
                }
                else
                {
                    return Content("Xác nhận mật khẩu không khớp nhau!");
                }
            }
            return Content("Đăng ký thất bại");
        }
        
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            string username = f["Username"];
            string pass = f["Password"];
            string passMD5 = HashMD5(pass);
            ThanhVien temp = db.ThanhViens.SingleOrDefault(n => n.UserName == username && n.PassWord == passMD5);
            if (temp != null)
            {
                Session.Add("dangnhap", temp);
                if (temp.MaTV == 3)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            return RedirectToAction("DangNhap");
        }
        public ActionResult DangXuat()
        {
            Session["dangnhap"] = null;
            return RedirectToAction("Index");
        }
        public ActionResult Chat()
        {
            return View();
        }
        public string HashMD5(string password)
        {
            MD5 mh = MD5.Create();
            //Chuyển kiểu chuổi thành kiểu byte
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
            //mã hóa chuỗi đã chuyển
            byte[] hash = mh.ComputeHash(inputBytes);
            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}