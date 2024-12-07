using System.ComponentModel.DataAnnotations;

namespace Medhya.Admin.Models
{
    public class StockTransactions
    {
        public int? TransactionID { get; set; }
        [Required(ErrorMessage = "Item is required.")]
        public int? FK_ItemId { get; set; }
        [Required(ErrorMessage = "Vendor is required.")]
        public int? fk_VendorId { get; set; }
        [Required(ErrorMessage = "Transaction Type is required.")]
        public string? TransactionType { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int? ItemQty { get; set; }
        public int FK_OrderId { get; set; }
        public string? Reason { get; set; }
        public string? ItemName { get; set; } 
        public string? VendorName { get; set; }
        [Required(ErrorMessage = "UOM is required.")]
        public string UOM { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class StockTransactionViewModel
    {
        public StockTransactions NewTransaction { get; set; } = new StockTransactions();
        public IEnumerable<StockTransactions> Transactions { get; set; } = new List<StockTransactions>();
    }
}
