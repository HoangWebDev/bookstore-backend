namespace BookStore.IRepository
{
    public interface IStatisticRepository
    {
        public Task<Dictionary<string, int>> GetStatistics(List<string> tableNames);

        public Task<Dictionary<string, int>> GetStatistic(string tableName);
    }
}
