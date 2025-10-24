using System.Data;

namespace Data.IFactory
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateDBConnection();
    }
}
