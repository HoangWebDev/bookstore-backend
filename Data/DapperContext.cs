using Microsoft.Data.SqlClient;
using System.Data;

namespace BookStore.Data
{
    public class DapperContext
    {        
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {     
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                    ?? throw new ArgumentNullException("Connection string is null");
        }        

        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open(); //Mở kết nối khi gọi
            return connection;
        }
    }
}
