namespace Medhya.API.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int FK_CategoryId { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? FK_UOM { get; set; }
        public string? ItemStatus { get; set; }
        public string? Description { get; set; }

        public string? BarCode { get; set; }
        public decimal? ItemPrice { get; set; }
        public int VendorId { get; set; }
        public float? CGST { get; set; }
        public float? SGST { get; set; }
        public float Tax1 { get; set; }
        public float Tax2 { get; set; }
        public float Tax3 { get; set; }
        public float Tax4 { get; set; }

        // New properties added based on stored procedure
        public DateTime? CreatedDate { get; set; } // Represents I.CreatedDate
        public string? CreatedBy { get; set; }    // Represents I.CreatedBy
        public DateTime? UpdatedDate { get; set; } // Represents I.UpdatedDate
        public string? UpdatedBy { get; set; }    // Represents I.UpdatedBy

        public DateTime? PriceDate { get; set; }   // Represents P.PriceDate
        public decimal? Price { get; set; }       // Represents P.Price
        public decimal? VendorPrice { get; set; } // Represents P.VendorPrice
        public string? PriceStatus { get; set; }  // Represents P.PriceStatus
        public DateTime? PriceCreatedDate { get; set; } // Represents P.CreatedDate AS PriceCreatedDate
        public string? PriceCreatedBy { get; set; }    // Represents P.CreatedBy AS PriceCreatedBy
        public string? PriceUpdatedBy { get; set; }    // Represents P.UpdatedBy AS PriceUpdatedBy
        public DateTime? PriceUpdatedDate { get; set; } // Represents P.UpdatedDate AS PriceUpdatedDate
    }

    public class ItemListbyCategory
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<ItemList>? items { get; set; }
    }
    public class ItemList
    {
        public int ItemId { get; set; }
        public int CategoryId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemStatus { get; set; }
        public string? Description { get; set; }
        public List<UOMList>? UOMs { get; set; } = new List<UOMList>();
        public bool IsSoldOut { get; set; }

    }
    public class UOMList
    {
        public int ItemId { get; set; }
        public string? UOM { get; set; }
        public decimal Price { get; set; }
    }
}
