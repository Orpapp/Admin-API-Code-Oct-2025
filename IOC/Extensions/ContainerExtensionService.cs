using Business.IServices;
using Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOC.Extensions
{
    public static class ContainerExtensionService
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterRepositories(configuration);
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IManageService, ManageService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ICategoryServices, CategoryService>();
            services.AddScoped<IHangFireService, HangFireService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<IRewardService, RewardService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<IProgressService, ProgressService>();
            services.AddScoped<ILevelVoucherService, LevelVoucherService>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IHelpAndTipsService, HelpAndTipsService>();
            services.AddScoped<ISendTasksService, SendTasksService>();

        }
    }
}
