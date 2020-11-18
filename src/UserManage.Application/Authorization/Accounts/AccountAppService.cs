using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Extensions;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using UserManage.AbpCompanyCore;
using UserManage.Authorization.Accounts.Dto;
using UserManage.Authorization.Users;

namespace UserManage.Authorization.Accounts
{
    public class AccountAppService : UserManageAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AccountAppService(
            UserRegistrationManager userRegistrationManager, IPasswordHasher<User> passwordHasher)
        {
            _userRegistrationManager = userRegistrationManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var company = ObjectMapper.Map<AbpCompany>(input.CompanyInput);
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.PhoneNumber,
                // input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true,
                company
                
                // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }
        /// <summary>
        /// 设置重置验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SendPasswordResetCodeOutput> SendPasswordResetCode(SendPasswordResetCodeInput input)
        {
            var user = await GetUserByChecking(input.UserName);
            //if (user.PhoneNumber!=input.PhoneOrEmail&&user.EmailAddress!=input.PhoneOrEmail)
            //{

            //    throw new UserFriendlyException("邮箱或手机号码不正确", "邮箱或手机号码不正确");
            //}
            if (input.PhoneOrEmail.Contains("@"))
            {
                user.EmailAddress = input.PhoneOrEmail;
            }
            else
            {
                user.PhoneNumber = input.PhoneOrEmail;
            }

            user.PasswordResetCode = input.ResetCode;
            await UserManager.UpdateAsync(user);
            return ObjectMapper.Map<SendPasswordResetCodeOutput>(user);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            //if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            //{
            //    throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
            //}

            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        //用户名修改密码
        public async Task<ResetPasswordOutput> UserResetPassword(ResetPasswordInput input)
        {
            var user = await UserManager.FindByNameAsync(input.UserName);
            //if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            //{
            //    throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
            //}

            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        private async Task<User> GetUserByChecking(string userName)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UserFriendlyException("账号无效");
            }

            return user;
        }
    }
}
