using Data.Factory;
using Data.IFactory;
using Data.IRepository;
using Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOC.Extensions
{
    public static class ContainerExtensionRepository
    {
        public static void RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("OrpDbConnection");
            if (string.IsNullOrEmpty(dbConnectionString)) throw new Exception("Empty Database connection string");
            services.AddTransient(typeof(IRepositoryConfiguration), ctx => new RepositoryConfiguration(dbConnectionString));
            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
            services.AddTransient<IAccountRepositry, AccountRepository>();
            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IProfileRepository, ProfileRepository>();
            services.AddTransient<IHangFireRepository, HangFireRepository>();
            services.AddTransient<IFriendRepository, FriendRepository>();
            services.AddTransient<IRewardRepository, RewardRepository>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<ILevelRepository, LevelRepository>();
            services.AddTransient<IProgressRepository, ProgressRepository>();
            services.AddTransient<ILevelVoucherRepository, LevelVoucherRepository>();
            services.AddTransient<IShopRepository, ShopRepository>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
            services.AddTransient<ISubscriptionProductRepository, SubscriptionProductRepository>();
            services.AddTransient<IHelpAndTipsRepository, HelpAndTipsRepository>();
            services.AddTransient<ISendTasksRepository, SendTasksRepository>();
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
    }
}
