using Dapper;
using Medhya.API.Context;
using Medhya.API.Model;
using Medhya.API.Models;
using Medhya.API.Repository;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace Medhya.API.Services

{
    public class ItemRepository : IItemRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly ILogger<ItemRepository> _logger;
        public ItemRepository(DapperContext dapperContext, ILogger<ItemRepository> logger)
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }
        public async Task<int> AddAsync(Item item)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", item.Id);
            parameters.Add("ItemCode", item.ItemCode);
            parameters.Add("ItemName", item.ItemName);
            parameters.Add("FK_CategoryId", item.FK_CategoryId);
            parameters.Add("FK_UOM", item.FK_UOM);
            parameters.Add("ItemStatus", item.ItemStatus);
            parameters.Add("ItemPrice", item.ItemPrice);
            parameters.Add("Description", item.Description);
            parameters.Add("CGST", item.CGST);
            parameters.Add("SGST", item.SGST);
            parameters.Add("Tax1", item.Tax1);
            parameters.Add("Tax2", item.Tax2);
            parameters.Add("Tax3", item.Tax3);
            parameters.Add("Tax4", item.Tax4);
            parameters.Add("output", dbType: DbType.Int32, direction: ParameterDirection.Output);
            using (var connection = _dapperContext.CreateConnection())
            {

                await connection.QueryAsync("USP_MANAGE_ITEMDETAILS", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("output");
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("output", dbType: DbType.Int32, direction: ParameterDirection.Output);
            using (var connection = _dapperContext.CreateConnection())
            {
                await connection.ExecuteAsync("USP_DEL_ITEM", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("output");
            }
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<Item>("USP_LIST_ITEM", commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        public async Task<IEnumerable<ItemListbyCategory>> GetAllCategorieswithItems()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                var categoryDictionary = new Dictionary<int, ItemListbyCategory>();
                var itemDictionary = new Dictionary<int, ItemList>();
                var sql = "USP_GET_ITEMSBYCATEGORY";
                var categories = await connection.QueryAsync<ItemListbyCategory, ItemList, UOMList, ItemListbyCategory>(
                    sql,
                    (category, item, uom) =>
                    {
                        // Manage Category Mapping
                        if (!categoryDictionary.TryGetValue(category.CategoryId, out var existingCategory))
                        {
                            existingCategory = category;
                            categoryDictionary.Add(existingCategory.CategoryId, existingCategory);
                        }
                        if (existingCategory.items == null)
                        {
                            existingCategory.items = new List<ItemList>();
                        }
                        if (item != null)
                        {
                            if (!itemDictionary.TryGetValue(item.ItemId, out var existingItem))
                            {
                                existingItem = item;
                                itemDictionary.Add(existingItem.ItemId, existingItem);
                                existingCategory.items.Add(existingItem); // Link to Category
                            }
                            
                            // Manage UOM Mapping
                            if (uom != null)
                            {
                                existingItem.UOMs.Add(uom);
                            }
                        }

                        return category;
                    },
                    splitOn: "ItemId,UOM" // Split points for mapping
                );
                return categoryDictionary.Values.ToList();
            }
        }
        public async Task<Item> GetByIdAsync(int id)
        {
            using (var connection = _dapperContext.CreateConnection())
            {

                var result = await connection.QueryFirstOrDefaultAsync<Item>("USP_ITEMDETAILS_ID", new { Id = id }, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        //public Task<int> UpdateAsync(Item product)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
