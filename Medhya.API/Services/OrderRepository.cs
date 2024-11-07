using Dapper;
using Medhya.API.Context;
using Medhya.API.Model;
using Medhya.API.Models;
using Medhya.API.Repository;
using System.Data;

namespace Medhya.API.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<OrderRepository> _logger;
        public OrderRepository(IDbConnection dbConnection, ILogger<OrderRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }
        public async Task<int> AddAsync(TempOrder order)
        {
            var tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("FK_TempOrderId", typeof(int)));
            tvp.Columns.Add(new DataColumn("FK_ItemId", typeof(int)));
            tvp.Columns.Add(new DataColumn("ItemPrice", typeof(float)));
            tvp.Columns.Add(new DataColumn("ItemQty", typeof(int)));
            tvp.Columns.Add(new DataColumn("TotalItemPrice", typeof(float)));
            tvp.Columns.Add(new DataColumn("CGST", typeof(float)));
            tvp.Columns.Add(new DataColumn("SGST", typeof(float)));
            foreach (var item in order.Items)
            {
                var row = tvp.NewRow();
                row["FK_TempOrderId"] = item.FK_TempOrderId;
                row["FK_ItemId"] = item.FK_ItemId;
                row["ItemPrice"] = item.ItemPrice;
                row["ItemQty"] = item.ItemQty;
                row["ItemTotalAmount"] = item.ItemQty * item.ItemPrice;
                row["DiscountPrice"] = item.DiscountPrice;
                row["CGST"] = item.CGST;
                row["SGST"] = item.SGST;
                tvp.Rows.Add(row);
            }
            var parameters = new DynamicParameters();
            parameters.Add("Id", order.Id);
            parameters.Add("userId", order.userId);
            parameters.Add("ItemCount", order.ItemCount);
            parameters.Add("OrderAmount", order.OrderAmount);
            parameters.Add("ItemsTVP", tvp.AsTableValuedParameter("OrderItemsTableType"));
            parameters.Add("output", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbConnection.ExecuteAsync("USP_MANAGE_CREATETEMPORDER", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("output");

        }

        public Task<int> AddAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TempOrder> GetTempOrderById(int tempOrderId)
        {
            try
            {
                var storedProcedure = "dbo.USP_LIST_CATEGORY";
                var result = await _dbConnection.QueryAsync<TempOrder>(storedProcedure, new { tempOrderId }, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
