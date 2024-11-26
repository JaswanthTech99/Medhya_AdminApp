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
        public List<TempOrderItems>? Items { get; set; }

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
        public int Id { get; set; } // Order ID (Primary key)
        public string? OrderId { get; set; } // Optional, if you want a string-based unique order ID
        public string? UserId { get; set; } // User ID (foreign key)
        public int ItemCount { get; set; } // Total number of items in the order
        public decimal OrderAmount { get; set; } // Total order amount (sum of item prices, taxes, etc.)
        public string? OrderStatus { get; set; } // Current status of the order (e.g., Pending, Completed)

        // Add the missing properties here:
        public decimal CGST { get; set; } // Central GST
        public decimal SGST { get; set; } // State GST
        public string? Status { get; set; } // Order status (e.g., "Pending", "Completed")
        public DateTime OrderDateTime { get; set; } // Order date and time
        public string? TransactionType { get; set; } // Transaction type (e.g., "Online", "Cash")
        public string? PaymentType { get; set; } // Payment type (e.g., "Credit", "Debit")
        public decimal DeliveryCharges { get; set; } // Delivery charges

        public List<OrderItems>? Items { get; set; } // List of items associated with the order
    }

    public class OrderItems
    {
        public int Id { get; set; } // Item ID (Primary key)
        public int OrderId { get; set; } // Order ID (foreign key)
        public int ItemId { get; set; } // Item ID (foreign key to an item catalog)
        public decimal ItemPrice { get; set; } // Price per unit of the item
        public int ItemCount { get; set; } // Quantity of the item
        public decimal ItemTotalAmount { get; set; } // Total amount for the item (ItemPrice * ItemCount)
        public decimal DiscountPrice { get; set; } // Discount applied on the item
        public decimal CGST { get; set; } // Central GST for the item
        public decimal SGST { get; set; } // Sta

    }
}
