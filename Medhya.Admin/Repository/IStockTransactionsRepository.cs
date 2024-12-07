using Medhya.Admin.Models;

namespace Medhya.Admin.Repository
{
    public interface IStockTransactionsRepository
    {
        Task<int> CreateTransactionAsync(StockTransactions transaction);
        Task<StockTransactions> GetTransactionByIdAsync(int transactionId);
        Task<IEnumerable<StockTransactions>> GetAllTransactionsAsync();
        Task<bool> DeleteTransactionAsync(int transactionId);
        Task<IEnumerable<DDLItem>> GetItemList();
        Task<IEnumerable<DDLVendor>> GetVendorList();
        Task<List<string>> UOMList();
    }
}
