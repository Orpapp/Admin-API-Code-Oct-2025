namespace Api.Helper
{
    public static class RegisterAuthorization
    {
        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(config =>
            {
                config.AddPolicy("AuthorisedUser", policy =>
                {
                    policy.RequireClaim("userId");
                });
            });
        }
    }
}
