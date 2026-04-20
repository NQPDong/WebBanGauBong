namespace WebBanGauBong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Rating")]
    public partial class Rating
    {
        public int RatingID { get; set; }

        public int? ProductID { get; set; }

        public int? UserID { get; set; }

        public int? Star { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }
        public int? ProductSizeName { get; set; }
        public DateTime RatingDate { get; set; }
        public virtual Product Product { get; set; }

        public virtual Users Users { get; set; }
    }
}
