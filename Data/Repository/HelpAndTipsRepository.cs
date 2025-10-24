using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class HelpAndTipsRepository : IHelpAndTipsRepository
    {
        private readonly IDbConnection _dbConnection;
        public HelpAndTipsRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<List<HelpAndTips>> GetHelpAndTips()
        {
            return (await _dbConnection.QueryAsync<HelpAndTips>("GetHelpAndTips", null, commandType: CommandType.StoredProcedure)).AsList();
        }
    }
}
