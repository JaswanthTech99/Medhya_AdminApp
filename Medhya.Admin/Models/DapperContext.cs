
using Microsoft.Data.SqlClient;
using System.Data;

using Microsoft.Extensions.Configuration;

namespace Medhya.Admin.Models
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuaraion)
        {
            _configuration = configuaraion;
            _connectionString = configuaraion.GetConnectionString("SqlConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
