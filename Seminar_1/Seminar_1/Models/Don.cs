//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Seminar_1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Don
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Don()
        {
            this.ChiTietDons = new HashSet<ChiTietDon>();
        }
    
        public int MaD { get; set; }
        public int MaNguoiMua { get; set; }
        public int MaNguoiBan { get; set; }
        public Nullable<System.DateTime> NgayDat { get; set; }
        public Nullable<System.DateTime> NgayGiao { get; set; }
        public int TrangThai { get; set; }
        public string TenNguoiMua { get; set; }
        public string TenNguoiBan { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDon> ChiTietDons { get; set; }
        public virtual ThanhVien ThanhVien { get; set; }
        public virtual ThanhVien ThanhVien1 { get; set; }
    }
}
