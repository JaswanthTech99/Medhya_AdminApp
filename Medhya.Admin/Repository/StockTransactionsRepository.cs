using Dapper;
using Medhya.Admin.Models;
using System.Data;

namespace Medhya.Admin.Repository
{
    public class StockTransactionsRepository : IStockTransactionsRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly DapperContext _dapperContext;
        public StockTransactionsRepository(IDbConnection dbConnection, DapperContext dapperContext)
        {
            _dbConnection = dbConnection;
            _dapperContext = dapperContext;
        }
        public async Task<int> CreateTransactionAsync(StockTransactions transaction)
        {
            var parameters = new DynamicParameters();
            if(transaction.TransactionID ==null)
            {
                transaction.TransactionID = 0;
            }
            parameters.Add("Id", transaction.TransactionID, DbType.Int32);
            parameters.Add("ItemID", transaction.FK_ItemId, DbType.Int32);
            parameters.Add("VendorID", transaction.fk_VendorId, DbType.Int32);
            parameters.Add("TransactionType", transaction.TransactionType, DbType.String);
            parameters.Add("Quantity", transaction.ItemQty, DbType.Int32);
            parameters.Add("Reason", transaction.Reason, DbType.String);
            parameters.Add("UOM", transaction.UOM, DbType.String);
            parameters.Add("CreatedBy", transaction.CreatedBy, DbType.String);
            parameters.Add("Output", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbConnection.ExecuteAsync("USP_INS_InsertStockTransaction", parameters, commandType: CommandType.StoredProcedure);
            int result= parameters.Get<int>("Output");
            return result;
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

        public async Task<StockTransactions> GetTransactionByIdAsync(int transactionId)
        {
            //var parameters = new { TransactionID = transactionId };
            var result = await _dbConnection.QueryFirstOrDefaultAsync<StockTransactions>("USP_GET_STOCKTRANSACTIONSBYID", new { transactionId },
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
        public async Task<List<string>> UOMList()
        {
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {

                    var uomList = await connection.QueryAsync<string>("USP_GET_UOM", commandType: CommandType.StoredProcedure);
                    return uomList.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
