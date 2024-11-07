namespace Medhya.API.Models
{
    
    public class TempOrder
    {
        public int Id { get; set; }
        public string? userId { get; set; }
        public int ItemCount { get; set; }
        public decimal OrderAmount { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
       // public DateTime? OrderTime { get; set; }
        public List<TempOrderItems>?Items { get; set; }
       
    }

    public class TempOrderItems
    {
        public int Id { get; set; }
        public int FK_TempOrderId { get; set; }
        public int FK_ItemId { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemQty { get; set; }
        public decimal ItemTotalAmount { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string? OrderId { get; set; }
        public string? userId { get; set; }
        public int ItemCount { get; set; }
        public decimal OrderAmount { get; set; }
        public string? OrderStatus { get; set; }


    }
    public class OrderItems
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemCount { get; set; }
        public decimal ItemTotalAmount { get; set; }
    }
}
