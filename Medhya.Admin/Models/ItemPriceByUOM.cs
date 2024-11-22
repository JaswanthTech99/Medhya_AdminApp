using System.ComponentModel.DataAnnotations;

namespace Medhya.Admin.Models
{
    public class ItemPriceByUOM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select an item.")]
        public int FK_ItemId { get; set; }
        public string? ItemName { get; set; }
        [Required]
        public string UOM { get; set; }
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter a valid price with up to 2 decimal places.")]
        public decimal Price { get; set; }
        public  string? Status { get; set; }
    }

    public class  ItemPricebyUOMViewModel
    {
        public ItemPriceByUOM? itemPriceByUOM { get; set; }
        public IEnumerable<ItemPriceByUOM>? itemPriceByUOMList { get; set; }
    }
}
