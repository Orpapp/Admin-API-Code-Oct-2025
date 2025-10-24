using Data.IFactory;
using System.Data;
using System.Data.SqlClient;

namespace Data.Factory
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IRepositoryConfiguration _repositoryConfiguration;

        public DbConnectionFactory(IRepositoryConfiguration repositoryConfiguration)
        {
            _repositoryConfiguration = repositoryConfiguration;
        }

        public IDbConnection CreateDBConnection()
        {
            return new SqlConnection(_repositoryConfiguration.GetDBConnectionString());
        }
    }
}
