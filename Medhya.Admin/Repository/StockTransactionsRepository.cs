using Dapper;
using Medhya.Admin.Models;
using System.Data;

namespace Medhya.Admin.Repository
{
    public class StockTransactionsRepository : IStockTransactionsRepository
    {
        private readonly IDbConnection _dbConnection;

        public StockTransactionsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> CreateTransactionAsync(StockTransactions transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("ItemID", transaction.FK_ItemId, DbType.Int32);
            parameters.Add("VendorID", transaction.fk_VendorId, DbType.Int32);
            parameters.Add("TransactionType", transaction.TransactionType, DbType.String);
            parameters.Add("Quantity", transaction.ItemQty, DbType.Int32);
            parameters.Add("Reason", transaction.Reason, DbType.String);
            parameters.Add("Output", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbConnection.ExecuteAsync("USP_INS_InsertStockTransaction", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("Output");
        }



        public Task<bool> DeleteTransactionAsync(int transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StockTransactions>> GetAllTransactionsAsync()
        {
            return await _dbConnection.QueryAsync<StockTransactions>(
                  "USP_LIST_STOCKTRANSACTIONS",
                  commandType: CommandType.StoredProcedure
              );
        }

        public async Task<StockTransactions?> GetTransactionByIdAsync(int transactionId)
        {
            //var parameters = new { TransactionID = transactionId };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<StockTransactions>("GetStockTransactionByID", transactionId,
                 commandType: CommandType.StoredProcedure);
            return result;

        }
        public async Task<IEnumerable<DDLItem>> GetItemList()
        {
            var result = await _dbConnection.QueryAsync<DDLItem>("USP_DDL_ITEM", commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        public async Task<IEnumerable<DDLVendor>> GetVendorList()
        {
            var result = await _dbConnection.QueryAsync<DDLVendor>("USP_DDL_VENDORLIST", commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        
    }
}
