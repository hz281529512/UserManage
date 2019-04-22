using System.Threading.Tasks;
using UserManage.Configuration.Dto;

namespace UserManage.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
