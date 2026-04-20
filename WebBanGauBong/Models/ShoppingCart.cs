namespace WebBanGauBong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("ShoppingCart")]
    public partial class ShoppingCart
    {
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ShoppingCart()
        {
            ShoppingCartItem = new HashSet<ShoppingCartItem>();
        }

        public int ShoppingCartID { get; set; }

        public int? UserID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShoppingCartItem> ShoppingCartItem { get; set; }

        public virtual Users Users { get; set; }

        [NotMapped]
        public int TongSoLuong
        {
            get
            {
                return ShoppingCartItem.Sum(t => t.Quantity.Value);
            }
        }
        [NotMapped]
        public decimal TongThanhTien
        {
            get {
                return ShoppingCartItem.Sum(t => t.ThanhTien);
            }
        }


       
        
    }
}
