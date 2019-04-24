

using Abp.Domain.Services;

namespace UserManage
{
	public abstract class UserManageDomainServiceBase : DomainService
	{
		/* Add your common members for all your domain services. */
		/*在领域服务中添加你的自定义公共方法*/





		protected UserManageDomainServiceBase()
		{
			LocalizationSourceName = UserManageConsts.LocalizationSourceName;
		}
	}
}
