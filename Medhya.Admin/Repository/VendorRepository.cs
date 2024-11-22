using Dapper;
using Medhya.Admin.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Medhya.Admin.Repository
{
    public class VendorRepository : IVendorRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly DapperContext _dapperContext;
        public VendorRepository(IDbConnection dbConnection, DapperContext dapperContext)

        {
            _dbConnection = dbConnection;
            _dapperContext = dapperContext;
        }

        public  Vendor GetVendorById(int id)
        {
          var result =  _dbConnection.QueryFirstOrDefault<Vendor>("USP_Get_VendorById", new { id}, commandType: CommandType.StoredProcedure);
            return  result;
        }


        public async  Task<IEnumerable<Vendor>> GetVendors()
        {
            var list =await  _dbConnection.QueryAsync<Vendor>("USP_LIST_AllVendors", commandType: CommandType.StoredProcedure);
            return list.ToList();
        }

        public async Task<int> InsertVendor(Vendor vendor)
        {
            try
            {
                using IDbConnection dbconnection = _dbConnection;
                var parameters = new DynamicParameters();
                parameters.Add("Id", vendor.Id);
                parameters.Add("VendorName", vendor.VendorName);
                parameters.Add("ContactNumber", vendor.ContactNumber);
                parameters.Add("Address", vendor.Address);
                parameters.Add("City", vendor.City);
                parameters.Add("State", vendor.State);
                parameters.Add("Status", vendor.Status);
                parameters.Add("output", dbType: DbType.Int32, direction: ParameterDirection.Output);
                using (var connection = _dapperContext.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>("USP_MANAGE_Vendor", parameters, commandType: CommandType.StoredProcedure);
                }
                return parameters.Get<int>("output");
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}
