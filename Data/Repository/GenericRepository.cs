using Dapper;
using Data.IFactory;
using Data.IRepository;
using System.Data;

namespace Data.Repository
{

    public class GenericRepository<T>(IDbConnectionFactory dbConnection) : IGenericRepository<T>
    {

        private readonly IDbConnection _dbConnection = dbConnection.CreateDBConnection();


        public async Task<List<T>> GetAll()
        {

            return (await _dbConnection.QueryAsync<T>($"GetAll{typeof(T).Name}s", commandType: CommandType.StoredProcedure)).ToList();
        }
        public async Task<T> GetById(int id)
        {


            var parms = new
            {
                @Id = id
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<T>($"Get{typeof(T).Name}ById", parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddUpdate(T entity)
        {
            
            return await _dbConnection.ExecuteScalarAsync<int>($"AddUpdate{typeof(T).Name}", entity, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> Delete(int id)
        {

            var parms = new
            {
                @Id = id
            };
            try
            {
                return await _dbConnection.ExecuteAsync($"Delete{typeof(T).Name}ById", parms, commandType: CommandType.StoredProcedure);

            }
            catch (Exception)
            {
                return 0;
            }
        }


    }
}
