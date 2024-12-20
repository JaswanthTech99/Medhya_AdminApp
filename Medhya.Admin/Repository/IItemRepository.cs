﻿using Medhya.Admin.Models;
namespace Medhya.Admin.Repository
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(int id);
        Task<int> AddAsync(Item item);
        // Task<int> UpdateAsync(Item item);
        Task<int> DeleteAsync(int id);
        Task<List<string>> UOMList();
        Task<List<Category>> CategoryList();
        Task<IEnumerable<ItemPriceByUOM>> ItemPriceByUOMAsyncList();
        Task<int> AddItemPriceByUOM(ItemPriceByUOM itemPriceByUOM);
        Task<ItemPriceByUOM> GetItemPriceByUOMById(int id);
        Task<IEnumerable<DDLItem>> GetItemList();
    }
}
