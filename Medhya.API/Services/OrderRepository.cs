﻿using Dapper;
using Medhya.API.Context;
using Medhya.API.Model;
using Medhya.API.Models;
using Medhya.API.Repository;
using System.Data;

namespace Medhya.API.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(DapperContext dapperContext, ILogger<OrderRepository> logger)
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }
        public async Task<int> AddAsync(TempOrder order)
        {
            using var connection = _dapperContext.CreateConnection();

            var tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("FK_TempOrderId", typeof(int)));
            tvp.Columns.Add(new DataColumn("FK_ItemId", typeof(int)));
            tvp.Columns.Add(new DataColumn("ItemPrice", typeof(float)));
            tvp.Columns.Add(new DataColumn("ItemQty", typeof(int)));
            tvp.Columns.Add(new DataColumn("TotalItemPrice", typeof(float)));
            tvp.Columns.Add(new DataColumn("DiscountPrice", typeof(float)));

            tvp.Columns.Add(new DataColumn("FK_UOM", typeof(string)));
            tvp.Columns.Add(new DataColumn("ItemStatus", typeof(string)));


            tvp.Columns.Add(new DataColumn("CGST", typeof(float)));
            tvp.Columns.Add(new DataColumn("SGST", typeof(float)));
            foreach (var item in order.Items)
            {
                var row = tvp.NewRow();
                row["FK_TempOrderId"] = item.FK_TempOrderId;
                row["FK_ItemId"] = item.FK_ItemId;
                row["ItemPrice"] = item.ItemPrice;
                row["ItemQty"] = item.ItemQty;
                row["ItemStatus"] = item.ItemStatus;
                row["FK_UOM"] = item.FK_UOM;
                row["TotalItemPrice"] = item.ItemQty * item.ItemPrice;
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
            parameters.Add("TotalItemPrice", order.OrderAmount); // Added to match SP's TotalItemPrice
            parameters.Add("TransactionType", order.TransactionType); // Mapped to @TransactionType
            parameters.Add("PaymentType", order.PaymentType); // Mapped to @PaymentType
            parameters.Add("DeliveryCharges", order.DeliveryCharges); // Added DeliveryCharges
            parameters.Add("Status", order.OrderStatus); // Added DeliveryCharges

            parameters.Add("ItemsTVP", tvp.AsTableValuedParameter("OrderItemsTableType"));
            parameters.Add("output", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("USP_MANAGE_CREATETEMPORDER", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("output");

         }




        public async Task<int> AddAsync(Order order)
        {
            using var connection = _dapperContext.CreateConnection();

            try
            {



                // Create DataTable for Table-Valued Parameter (TVP)

                var tvp = new DataTable();
                tvp.Columns.Add(new DataColumn("FK_OrderId", typeof(int))); // Matches INT
                tvp.Columns.Add(new DataColumn("FK_ItemId", typeof(int))); // Matches INT
                tvp.Columns.Add(new DataColumn("ItemPrice", typeof(decimal))); // Matches MONEY
                tvp.Columns.Add(new DataColumn("ItemQty", typeof(int))); // Matches INT
                tvp.Columns.Add(new DataColumn("TotalItemPrice", typeof(decimal))); // Matches MONEY
                tvp.Columns.Add(new DataColumn("DiscountPrice", typeof(decimal))); // Matches MONEY
                tvp.Columns.Add(new DataColumn("FK_UOM", typeof(string))); // Matches VARCHAR/CHAR
                tvp.Columns.Add(new DataColumn("ItemStatus", typeof(string))); // Matches VARCHAR/CHAR
                tvp.Columns.Add(new DataColumn("CGST", typeof(decimal))); // Matches MONEY
                tvp.Columns.Add(new DataColumn("SGST", typeof(decimal))); // Matches MONEY

                //    var tvp = new DataTable();

                //// Define columns to match the parameters expected by the stored procedure
                //tvp.Columns.Add(new DataColumn("FK_OrderId", typeof(int))); // ItemId should match the column in the stored procedure   FK_ItemId
                //tvp.Columns.Add(new DataColumn("FK_ItemId", typeof(int))); // ItemId should match the column in the stored procedure   

                //tvp.Columns.Add(new DataColumn("ItemPrice", typeof(decimal))); // Changed from float to decimal to match model
                //tvp.Columns.Add(new DataColumn("ItemQty", typeof(int))); // ItemQty column (added based on the order model)
                //tvp.Columns.Add(new DataColumn("TotalItemPrice", typeof(decimal))); // TotalItemPrice, renamed to match stored procedure
                //    tvp.Columns.Add(new DataColumn("DiscountPrice", typeof(float)));

                //    tvp.Columns.Add(new DataColumn("FK_UOM", typeof(string)));
                //    tvp.Columns.Add(new DataColumn("ItemStatus", typeof(string)));

                //    tvp.Columns.Add(new DataColumn("CGST", typeof(decimal))); // Central GST
                //tvp.Columns.Add(new DataColumn("SGST", typeof(decimal))); // State GST

                // Loop through the items in the order and populate the TVP
                foreach (var item in order.Items)
                {
                    var row = tvp.NewRow();
                    row["FK_OrderId"] = item.FK_OrderId; // Match the ItemId property
                    row["FK_ItemId"] = item.FK_ItemId; // Match the ItemId property

                    row["ItemPrice"] = item.ItemPrice; // Set ItemPrice
                    row["ItemQty"] = item.ItemCount; // Set ItemQty
                    row["ItemStatus"] = item.ItemStatus;

                    row["FK_UOM"] = item.FK_UOM;
                    row["TotalItemPrice"] = item.ItemQty * item.ItemPrice;
                    row["DiscountPrice"] = item.DiscountPrice;
                    row["CGST"] = item.CGST; // Set CGST
                    row["SGST"] = item.SGST; // Set SGST
                    tvp.Rows.Add(row);
                }

                // Setup parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("ID", order.Id); // Order ID (primary key)
                parameters.Add("UserId", order.UserId); // User ID (foreign key)
                parameters.Add("OrderAmount", order.OrderAmount); // Total Order Amount
                parameters.Add("CGST", order.CGST); // CGST value
                parameters.Add("SGST", order.SGST); // SGST value
                parameters.Add("Status", order.Status); // Order Status
                parameters.Add("ItemCount", order.ItemCount); // Number of items in the order
                parameters.Add("OrderDateTime", order.OrderDateTime); // Order Date and Time
                parameters.Add("TransactionType", order.TransactionType); // Transaction Type (Online, Cash, etc.)
                parameters.Add("PaymentType", order.PaymentType); // Payment Type (Credit, Debit, etc.)
                parameters.Add("DeliveryCharges", order.DeliveryCharges); // Delivery Charges
                parameters.Add("ItemsTVP", tvp.AsTableValuedParameter("OrderItemsTableType")); // TVP parameter for order items
                parameters.Add("Output", dbType: DbType.Int32, direction: ParameterDirection.Output); // Output parameter to capture success/failure

                // Execute stored procedure
                await connection.ExecuteAsync(
                    "USP_MANAGE_CREATEORDER", // Stored Procedure Name
                    parameters,
                    commandType: CommandType.StoredProcedure // Specify that it's a stored procedure
                );

                // Return the output value indicating success or failure
                return parameters.Get<int>("Output");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating order.");
                throw; // Rethrow the exception so it can be handled further up the stack
            }
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

        public async Task<(List<Order>, List<OrderItems>)> GetOrderDetailsByUserIdAsync(string userId)
        {
            using var connection = _dapperContext.CreateConnection();

            try
            {
                // Define the parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId);

                // Execute the stored procedure to get order details
                var orderDetailsQuery = "USP_GET_ORDERDETAILSBYUSERID";
                using (var multi = await connection.QueryMultipleAsync(orderDetailsQuery, parameters, commandType: CommandType.StoredProcedure))
                {
                    // Fetch the order details and order items
                    var orderDetails = (await multi.ReadAsync<Order>()).ToList();
                    var orderItems = (await multi.ReadAsync<OrderItems>()).ToList();

                    return (orderDetails, orderItems); // Return the results as a tuple
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (you can log the error if needed)
                throw new Exception("Error while fetching order details", ex);
            }
        }

        public async Task<(TempOrder, List<TempOrderItems>)> GetTempOrderDetailsByIdAsync(int tempOrderId)
        {
            using var connection = _dapperContext.CreateConnection();

            try
            {
                // Define the parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("tempOrderId", tempOrderId);

                // Execute the stored procedure
                var storedProcedure = "dbo.USP_GET_TEMPORDERDETAILSBYID";
                using (var multi = await connection.QueryMultipleAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure))
                {
                    // Fetch the TempOrder details
                    var tempOrder = (await multi.ReadAsync<TempOrder>()).FirstOrDefault();

                    // Fetch the TempOrderItems
                    var tempOrderItems = (await multi.ReadAsync<TempOrderItems>()).ToList();

                    return (tempOrder, tempOrderItems);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching TempOrder with ID: {tempOrderId}");
                throw new Exception("Error while fetching temp order details.", ex);
            }
        }

       
    }
}
