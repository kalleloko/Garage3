using Garage3.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Garage3.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> SeedDataAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                try
                {
                    var users = await UserSeedData.Init(context, services);
                    GarageSeeder.Seed(context, users);
                }
                catch (Exception)
                {
                    throw;
                }
                return app;


            }

        }

    }
}
