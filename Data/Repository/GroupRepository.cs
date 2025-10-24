using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.Request.Group;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using System.Data;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        private readonly IDbConnection _dbConnection;
        public GroupRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<int> AddUpdateGroup(Group request)
        {
            var parms = new DynamicParameters();
            parms.Add("@Id", request.Id);
            parms.Add("@CreatedBy", request.CreatedBy);
            parms.Add("@Title", request.Title);
            parms.Add("@Attachment", request.Attachment);
            parms.Add("@Description", request.Description);
            parms.AddTable("@ParticipantsId", "ParticipantsType", request.ParticipantsId);
            return await _dbConnection.ExecuteScalarAsync<int>("AddUpdateGroup", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> DeleteGroup(GroupId request)
        {
            return await _dbConnection.ExecuteAsync("DeleteGroup", param: request, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> LeftGroup(Participants request,int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @GroupId = request.Id,
                @ParticipantId = userId

            });
            return await _dbConnection.ExecuteAsync("DeleteGroupParticipant", parameters, commandType: CommandType.StoredProcedure);
         
        }

        public async Task<List<GroupsDetails>> GetGroups(GetGroups request)
        {
            return (await _dbConnection.QueryAsync<GroupsDetails>("GroupsDetails", param: request, commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<(List<Post> post, List<PostImages> images)> GetGroupPosts(GroupPosts request)
        {
            using (var result = await _dbConnection.QueryMultipleAsync("[GroupPosts]", param: request, commandType: CommandType.StoredProcedure))
            {
                var posts = (await result.ReadAsync<Post>()).ToList();
                var images = (await result.ReadAsync<PostImages>()).ToList();
                _dbConnection.Dispose();
                return (posts, images);
            }
        }
        public async Task<List<ParticipantsDetails>> GetParticipants(GroupId request)
        {
            return (await _dbConnection.QueryAsync<ParticipantsDetails>("GroupParticipantsDetail", param: request, commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<GroupDetailsWithParticipants> GetGroupWithParticipants(GroupId request,int userId)
        {
           
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @GroupId = request.Id,
                @UserId = userId
                
            });
            using (var result = await _dbConnection.QueryMultipleAsync("GetGroupWithParticipants", parameters, commandType: CommandType.StoredProcedure))
            {
                var group = await result.ReadFirstOrDefaultAsync<GroupDetailsWithParticipants>();
                if (group is not null)
                {
                    group.ParticipantsDetails = (await result.ReadAsync<ParticipantsDetails>()).ToList();
                }

                return group ?? new();
            }
        }

        public async Task<int> DeleteGroupParticipant(DeleteGroupParticipantRequestModel request)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @GroupId = request.GroupId,
                @ParticipantId = request.ParticipantId

            });
            return await _dbConnection.ExecuteAsync("DeleteGroupParticipant", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<ParticipantsDetails>> GetNotAddedParticipants(GetNotAddedParticipantsRequestModel request,int userId)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.AddDynamicParams(new
                {
                    @GroupId = request.Id,
                    @Search = request.SearchTerm,
                    @UserId = userId
                });
                return (await _dbConnection.QueryAsync<ParticipantsDetails>("GetNotAddedParticipants", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new List<ParticipantsDetails>();
            }
        }

        public async Task<int> AddParticipants(AddParticipantsRequestModel request)
        {

            try
            {
                var parms = new DynamicParameters();
                parms.Add("@GroupId", request.GroupId);
                parms.AddTable("@ParticipantsId", "ParticipantsType", request.ParticipantsId);
               // return await _dbConnection.ExecuteScalarAsync<int>("AddParticipants", param: parms, commandType: CommandType.StoredProcedure);
                return await _dbConnection.ExecuteAsync("AddParticipants", parms, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }

        }


    }
}



