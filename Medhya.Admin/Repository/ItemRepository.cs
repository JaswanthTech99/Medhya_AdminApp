using Dapper;
using System.Data;
using Medhya.Admin.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;

namespace Medhya.Admin.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly DapperContext _dapperContext;
        private readonly ILogger<ItemRepository> _logger;
        private readonly IConfiguration configuration;
        public ItemRepository(IConfiguration _configuration, IDbConnection dbConnection, ILogger<ItemRepository> logger)
        {

            _dbConnection = dbConnection;
            configuration = _configuration;
            _logger = logger;
        }
        public string ConnString
        {
            get
            {
                return this.configuration.GetConnectionString("SqlConnection");
            }
        }
        public async Task<int> AddAsync(Item item)
        {
            try
            {
                using var con = new SqlConnection(ConnString);
                var parameters = new DynamicParameters();
                parameters.Add("Id", item.Id);
                parameters.Add("ItemCode", item.ItemCode);
                parameters.Add("ItemName", item.ItemName);
                parameters.Add("FK_CategoryId", item.FK_CategoryId);
                //parameters.Add("FK_UOM", item.FK_UOM);
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


                await con.QueryAsync("USP_MANAGE_ITEMDETAILS", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("output");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public async Task<List<Category>> CategoryList()
        {
            try
            {
                var uomList = await _dbConnection.QueryAsync<Category>("USP_LIST_CATEGORY", commandType: CommandType.StoredProcedure);
                return uomList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public async Task<ItemPriceByUOM> GetItemPriceByUOMById(int id)
        {
            try
            {
                var result = await _dbConnection.QueryFirstOrDefaultAsync<ItemPriceByUOM>("USP_GET_ITEMPRICEBYUOMBYID", new { Id = id }, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<DDLItem>> GetItemList()
        {
            var result = await _dbConnection.QueryAsync<DDLItem>("USP_DDL_ITEM", commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        public async Task<IEnumerable<ItemPriceByUOM>> ItemPriceByUOMAsyncList()
        {
            try
            {
                var result = await _dbConnection.QueryAsync<ItemPriceByUOM>("USP_LIST_ITEMPRICEBYUOM", commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<int> AddItemPriceByUOM(ItemPriceByUOM itemPriceByUOM)
        {
            try
            {
                using var con = new SqlConnection(ConnString);
                var parameters = new DynamicParameters();
                parameters.Add("Id", itemPriceByUOM.Id);
                parameters.Add("FK_ItemId", itemPriceByUOM.FK_ItemId);
                parameters.Add("FK_UOMId", itemPriceByUOM.UOM);
                parameters.Add("Price", itemPriceByUOM.Price);
                parameters.Add("Status", itemPriceByUOM.Status);
                parameters.Add("Output", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await con.QueryAsync("USP_INS_ITEMPRICEBYUOM", parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("Output");


            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<string>> UOMList()
        {
            try
            {
                var uomList = await _dbConnection.QueryAsync<string>("USP_GET_UOM", commandType: CommandType.StoredProcedure);
                return uomList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public Task<int> UpdateAsync(Item product)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

