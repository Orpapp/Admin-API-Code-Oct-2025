using Data.IFactory;

namespace Data.Factory
{
    public class RepositoryConfiguration : IRepositoryConfiguration
    {
        private readonly string _dbConnectionString;

        public RepositoryConfiguration(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }

        public string GetDBConnectionString()
        {
            return _dbConnectionString;
        }
    }
}
