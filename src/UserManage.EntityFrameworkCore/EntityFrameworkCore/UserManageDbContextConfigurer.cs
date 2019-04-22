using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace UserManage.EntityFrameworkCore
{
    public static class UserManageDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<UserManageDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<UserManageDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
