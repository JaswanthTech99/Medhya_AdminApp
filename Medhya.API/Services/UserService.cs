using Dapper;
using Medhya.API.Context;
using Medhya.API.Model;
using Medhya.API.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

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
                    _param.Add("Output", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    _param.Add("UserId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    const string storedProcedure = "dbo.USP_Manage_Cinema";
                    await connection.QueryAsync<int>(storedProcedure, _param, commandType: CommandType.StoredProcedure);
                    ResponeMessage result = new()
                    {
                        ErrorNo = _param.Get<int>("Output"),
                        Message = _param.Get<int>("UserId").ToString()
                    };
                    return await Task.FromResult(result);
                }
               
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<int> UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
