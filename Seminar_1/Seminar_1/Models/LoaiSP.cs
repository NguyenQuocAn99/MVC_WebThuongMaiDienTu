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
    
    public partial class LoaiSP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LoaiSP()
        {
            this.SanPhams = new HashSet<SanPham>();
        }
    
        public int MaLSP { get; set; }
        public string TenLSP { get; set; }
        public Nullable<int> MaDM { get; set; }
    
        public virtual DanhMucSP DanhMucSP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
