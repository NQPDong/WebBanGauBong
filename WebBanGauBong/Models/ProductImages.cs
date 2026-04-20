namespace WebBanGauBong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductImages
    {
        [Key]
        public int ProductImageID { get; set; }

        public int? ProductID { get; set; }

        [StringLength(500)]
        public string ImageURL { get; set; }

        public virtual Product Product { get; set; }
    }
}
