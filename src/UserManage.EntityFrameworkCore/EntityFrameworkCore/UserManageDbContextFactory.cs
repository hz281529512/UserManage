using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using UserManage.Configuration;
using UserManage.Web;

namespace UserManage.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class UserManageDbContextFactory : IDesignTimeDbContextFactory<UserManageDbContext>
    {
        public UserManageDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<UserManageDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            UserManageDbContextConfigurer.Configure(builder, configuration.GetConnectionString(UserManageConsts.ConnectionStringName));

            return new UserManageDbContext(builder.Options);
        }
    }
}
