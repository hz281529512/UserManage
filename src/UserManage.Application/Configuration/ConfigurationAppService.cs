using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using UserManage.Configuration.Dto;

namespace UserManage.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : UserManageAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
