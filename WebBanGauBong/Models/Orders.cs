namespace WebBanGauBong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    public partial class Orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orders()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        [Key]
        public int OrderID { get; set; }

        public int? UserID { get; set; }

        public DateTime? OrderDate { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(50)]
        public string UserPaymentMethod { get; set; }

        [StringLength(20)]
        public string SDT { get; set; }

        [StringLength(60)]
        public string NameUser { get; set; }

        public decimal? Discount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }

        public virtual Users Users { get; set; }

        public decimal TamTinh()
        {
            return (decimal)OrderDetail.Sum(t => t.Quantity * t.UnitPrice);
        }
        
        public decimal ThanhTien() {
            return TamTinh() + Discount.GetValueOrDefault();
        }
        
       
    }
}
