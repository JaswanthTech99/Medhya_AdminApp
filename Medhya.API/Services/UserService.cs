using Dapper;
using Medhya.API.Context;
using Medhya.API.Model;
using Medhya.API.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;
using static Medhya.API.Model.User;

namespace Medhya.API.Services
{
    public class UserService : IUserRepository
    {
        private DapperContext _context;
        public UserService(DapperContext context) => _context = context;

        public async Task<ResponeMessage> CreateUser(User user)
        {            
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var _param = new DynamicParameters();
                    _param.Add("MobileNumber", user.MobileNumber);
                    _param.Add("OTP", user.OTP);
                    _param.Add("FullName", user.Name);
                    _param.Add("Email", user.Email);
                    _param.Add("UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    _param.Add("Status", dbType: DbType.Int32, direction: ParameterDirection.Output);


                    const string storedProcedure = "[dbo].[USP_INS_CREATEUSER]";
                    await connection.ExecuteAsync(storedProcedure, _param, commandType: CommandType.StoredProcedure);

                    int status = _param.Get<int>("Status");
                    int userId = _param.Get<int>("UserId");

                    // Build the response message
                    ResponeMessage result = new()
                    {
                        ErrorNo = _param.Get<int>("Status"),
                        Message = _param.Get<int>("UserId").ToString()
                    };

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponeMessage> InsertUserAddress(int userId, IEnumerable<UserAddress> addressDetails)
        {
            
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var table = new DataTable();
                    table.Columns.Add("FK_UserId", typeof(int));
                    table.Columns.Add("AddressLine1", typeof(string));
                    table.Columns.Add("Area", typeof(string));
                    table.Columns.Add("State", typeof(string));
                    table.Columns.Add("City", typeof(string));
                    table.Columns.Add("Longitude", typeof(decimal));
                    table.Columns.Add("Latitude", typeof(decimal));
                    table.Columns.Add("CreatedDate", typeof(DateTime));
                    table.Columns.Add("UpdatedDate", typeof(DateTime));

                    foreach (var address in addressDetails)
                    {
                        table.Rows.Add(
                            userId,
                            address.AddressLine1,
                            address.Area,
                            address.State,
                            address.City,
                            address.Longitude,
                            address.Latitude,
                            DateTime.Now,
                            DateTime.Now
                        );
                    }

                    var _param = new DynamicParameters();
                    _param.Add("UserId", userId, DbType.Int32);
                    _param.Add("AddressDetails", table.AsTableValuedParameter("UserAddressTableType"));

                    const string storedProcedure = "dbo.USP_INSERT_USER_ADDRESS";
                    await connection.ExecuteAsync(storedProcedure, _param, commandType: CommandType.StoredProcedure);

                    return new ResponeMessage
                    {
                        ErrorNo = 0,
                        Message = "Addresses inserted successfully."
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponeMessage> UpdateUserAddress(int addressId, int userId, IEnumerable<UserAddress> addressDetails)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var table = new DataTable();
                    table.Columns.Add("FK_UserId", typeof(int));
                    table.Columns.Add("AddressLine1", typeof(string));
                    table.Columns.Add("Area", typeof(string));
                    table.Columns.Add("State", typeof(string));
                    table.Columns.Add("City", typeof(string));
                    table.Columns.Add("Longitude", typeof(decimal));
                    table.Columns.Add("Latitude", typeof(decimal));
                    table.Columns.Add("CreatedDate", typeof(DateTime));
                    table.Columns.Add("UpdatedDate", typeof(DateTime));

                    foreach (var address in addressDetails)
                    {
                        table.Rows.Add(userId, address.AddressLine1, address.Area, address.State, address.City,
                            address.Longitude, address.Latitude, DateTime.Now, DateTime.Now);
                    }

                    var _param = new DynamicParameters();
                    _param.Add("Address_Id", addressId, DbType.Int32);
                    _param.Add("UserId", userId, DbType.Int32);
                    _param.Add("AddressDetails", table.AsTableValuedParameter("UserAddressTableType"));

                    const string storedProcedure = "dbo.USP_UPDATE_USER_ADDRESS";
                    await connection.ExecuteAsync(storedProcedure, _param, commandType: CommandType.StoredProcedure);

                    return new ResponeMessage
                    {
                        ErrorNo = 0,
                        Message = "Addresses updated successfully."
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponeMessage> UpdateUser(User user)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("USERID", user.UserId, DbType.Int32);
                    parameters.Add("MOBILENUMBER", user.MobileNumber, DbType.String, size: 15);
                    parameters.Add("OTP", user.OTP, DbType.Int32);
                    parameters.Add("Status", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    const string storedProcedure = "dbo.USP_UPD_USERMOBILE";

                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    ResponeMessage result = new()
                    {
                        ErrorNo = parameters.Get<int>("Status"),
                        Message = "Mobile Number updated successfully."
                    };
                    
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
