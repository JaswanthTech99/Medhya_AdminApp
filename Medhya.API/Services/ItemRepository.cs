using Dapper;
using Medhya.API.Models;
using Medhya.API.Repository;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
namespace Medhya.API.Services

{
    public class ItemRepository : IItemRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<ItemRepository> _logger;
        public ItemRepository(IDbConnection dbConnection, ILogger<ItemRepository> logger)
        {
            _dbConnection = dbConnection;
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
            

            await _dbConnection.QueryAsync("USP_MANAGE_ITEMDETAILS", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("output");

        }

        public async Task<int> DeleteAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("output", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await _dbConnection.ExecuteAsync("USP_DEL_ITEM", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("output"); ;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            var result = await _dbConnection.QueryAsync<Item>("USP_LIST_ITEM", commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            var result = await _dbConnection.QueryFirstOrDefaultAsync<Item>("USP_ITEMDETAILS_ID", new { Id = id }, commandType: CommandType.StoredProcedure);
            return result;
        }

        //public Task<int> UpdateAsync(Item product)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
