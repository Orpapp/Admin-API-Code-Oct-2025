using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using System.Data;
using System.Xml.Linq;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class PostRepository : GenericRepository<Posts>, IPostRepository
    {
        private readonly IDbConnection _dbConnection;
        public PostRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<int> AddPost(Posts request)
        {
            var parms = new DynamicParameters();
            parms.Add("@GroupId", request.GroupId);
            parms.Add("@CreatedBy", request.CreatedBy);
            parms.Add("@Title", request.Title);
            parms.AddTable("@Images", "ImagesType", request.PostImages);
            return await _dbConnection.ExecuteAsync("AddPost", param: parms, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> LikePost(Like request)
        {
            return await _dbConnection.ExecuteAsync("LikePost", param: request, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> PostComment(Comment request)
        {
            return await _dbConnection.ExecuteScalarAsync<int>("CommentPost", param: request, commandType: CommandType.StoredProcedure);
        }

        public async Task<Comments> GetComments(GetComments request)
        {

            using (var result = await _dbConnection.QueryMultipleAsync("[GetComments]", param: request, commandType: CommandType.StoredProcedure))
            {
                Comments comment = new();
                comment.AllComments = (await result.ReadAsync<AllComments>()).ToList();
                comment.Count = (await result.ReadAsync<int>()).FirstOrDefault();
               
                _dbConnection.Dispose();              
                return comment;
            }
          
        }
        public async Task<int> DeletePost(PostId request)
        {
            return await _dbConnection.ExecuteAsync("DeletePost", param: request, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> DeleteComment(PostId request)
        {
            return await _dbConnection.ExecuteAsync("DeleteComment", param: request, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckUserExists(int userId,int groupId,int postId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @UserId = userId,
                @GroupId = groupId,
                @PostId = postId
            });
            var result = await _dbConnection.QueryFirstOrDefaultAsync<int>("CheckUserExists", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }

    }
}



