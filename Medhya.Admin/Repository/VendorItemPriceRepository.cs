using Dapper;
using Medhya.Admin.Models;
using System.Data;
using System.Data.Common;

namespace Medhya.Admin.Repository
{
    public class VendorItemPriceRepository : IVendorItemPriceRepository
    {


        private readonly DapperContext _dapperContext;
        public VendorItemPriceRepository(IDbConnection dbConnection, DapperContext dapperContext)

        {

            _dapperContext = dapperContext;
        }
        public async Task<VendorItemPricing> GetVendorItemPricingByIdAsync(int? id)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<VendorItemPricing>("USP_GET_VENDORITEMPRICINGBYID", new { id },
               commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<VendorItemPricing>> GetVendorItemPricingListAsync(int? vendorId = null, int? itemId = null)
        {
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {
                    var list = await connection.QueryAsync<VendorItemPricing>("USP_GET_VENDORITEMPRICINGLIST", new { vendorId, itemId }
                  ,
                  commandType: CommandType.StoredProcedure);
                    return list.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> UpsertVendorItemPricingAsync(VendorItemPricing vendorItemPricing)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", vendorItemPricing.Id, DbType.Int32);
            parameters.Add("@FK_VendorID", vendorItemPricing.FK_VendorID, DbType.Int32);
            parameters.Add("@FK_ItemID", vendorItemPricing.FK_ItemID, DbType.Int32);
            parameters.Add("@FK_UOM", vendorItemPricing.FK_UOM.Trim(), DbType.String);
            parameters.Add("@PricePerUOM", vendorItemPricing.PricePerUOM, DbType.Decimal);
            parameters.Add("@Output", dbType: DbType.Int32, direction: ParameterDirection.Output);
            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>("usp_MANAGE_VendorItemPricing", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }

        }
        public async Task<IEnumerable<DDLItem>> GetItemList()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<DDLItem>("USP_DDL_ITEM", commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
        }
        public async Task<IEnumerable<DDLVendor>> GetVendorList()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<DDLVendor>("USP_DDL_VENDORLIST", commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
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
