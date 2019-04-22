using Abp.Authorization;
using UserManage.Authorization.Roles;
using UserManage.Authorization.Users;

namespace UserManage.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
