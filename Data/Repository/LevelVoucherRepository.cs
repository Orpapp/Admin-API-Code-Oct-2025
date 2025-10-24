using Dapper;
using Data.IFactory;
using Data.IRepository; 
using Shared.Model.Request.Admin; 
using System.Data; 

namespace Data.Repository
{
    public class LevelVoucherRepository : ILevelVoucherRepository
    {
        private readonly IDbConnection _dbConnection;
        public LevelVoucherRepository(IDbConnectionFactory dbConnection) 
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }
        public async Task<VoucherModel> GetVoucher(int levelNumber)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @levelNumber = levelNumber, 
            });
            return await _dbConnection.QueryFirstOrDefaultAsync<VoucherModel>("GetVoucher",param: parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<List<VoucherModel>> GetAllVouchers()
        { 
            return (await _dbConnection.QueryAsync<VoucherModel>("GetAllVouchers", commandType: CommandType.StoredProcedure)).ToList();
        }
        public async Task<int> UpdateVoucherImage(VoucherModel voucherModel)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @Id = voucherModel.Id,
                @LevelNumber = voucherModel.LevelNumber,
                @ImagePath = voucherModel.ImagePath,
            });
            return await _dbConnection.ExecuteAsync("UpdateVoucherImage", param: parameters, commandType: CommandType.StoredProcedure);
        } 
    }
}
