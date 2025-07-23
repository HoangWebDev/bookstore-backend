using BookStore.Data;
using BookStore.IRepository;
using Dapper;
using System.Data;

namespace BookStore.Repository
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly DapperContext _context;

        public StatisticRepository(DapperContext context)
        {
            _context = context;
        }

        //Thống kê từng bảng
        public async Task<Dictionary<string, int>> GetStatistic(string tableName)
        {
            var result = new Dictionary<string, int>();

            using (var connection = _context.CreateConnection())
            {
                var resultRow = await connection.QueryFirstOrDefaultAsync<dynamic>("QuantityStatistics", new { TableName = tableName }, commandType: CommandType.StoredProcedure);

                if (resultRow != null)
                {
                    int count = Convert.ToInt32(resultRow.value); // Ép kiểu tường minh
                    result[tableName] = count;
                }
            }
            return result;
        }

        //Thống kê nhiều bảng
        public async Task<Dictionary<string, int>> GetStatistics(List<string> tableNames)
        {            
            var result = new Dictionary<string, int>();

            using(var connection = _context.CreateConnection())
            {
                foreach (var tableName in tableNames)
                {                    
                    var resultRow = await connection.QueryFirstOrDefaultAsync<dynamic>("QuantityStatistics", new { TableName = tableName }, commandType: CommandType.StoredProcedure);

                    if (resultRow != null)
                    {
                        int count = Convert.ToInt32(resultRow.value); // Ép kiểu tường minh
                        result[tableName] = count;
                    }
                }
            }
            return result;
        }            
    }
}
