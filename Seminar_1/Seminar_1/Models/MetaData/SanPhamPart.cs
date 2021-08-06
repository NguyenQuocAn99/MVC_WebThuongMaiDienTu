using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Seminar_1.Models
{
    [MetadataTypeAttribute(typeof(SanPhamMetadata))]
    public partial class SanPham
    {
        internal sealed class SanPhamMetadata
        {
            [Required(ErrorMessage = "Tên sản phẩm không được trống")]
            public string TenSP { get; set; }
            [Required(ErrorMessage = "Loại sản phẩm không được trống")]
            public int MaLoaiSP { get; set; }
            [Required(ErrorMessage = "Số lượng không được trống")]
            public int SoLuongTon { get; set; }
            [Required(ErrorMessage = "Giá không được trống")]
            public decimal Gia { get; set; }
            public string MoTaSP { get; set; }
            [Required(ErrorMessage = "Hình ảnh không được trống")]
            public string HinhAnh { get; set; }
        }
    }
}