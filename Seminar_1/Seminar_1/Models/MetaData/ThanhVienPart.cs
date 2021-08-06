using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Seminar_1.Models
{
    [MetadataTypeAttribute(typeof(ThanhVienMetadata))]
    public partial class ThanhVien
    {
        internal sealed class ThanhVienMetadata
        {
            [Required(ErrorMessage = "Tên thành viên không được bỏ trống")]
            public string TenTV { get; set; }
            [Required(ErrorMessage = "Tài khoản không được bỏ trống")]
            public string UserName { get; set; }
            [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
            [StringLength(50, ErrorMessage = "Mật khẩu phải trên {2}", MinimumLength = 6)]
            public string PassWord { get; set; }
            public decimal Money { get; set; }
            [Required(ErrorMessage = "Bạn phải cung cấp địa chỉ")]
            public string DiaChi { get; set; }
            [Required(ErrorMessage = "Bạn phải cung cấp số điện thoại")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Đây không phải là số điện thoại")]
            public string SDT { get; set; }
        }
    }
}