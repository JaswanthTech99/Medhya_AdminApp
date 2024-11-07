namespace Medhya.Admin.Models
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
    }
}
