using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;

namespace UserManage.Models.TokenAuth
{
    public class ExternalAuthenticateModel
    {
        [Required]
        [StringLength(UserLogin.MaxLoginProviderLength)]
        public string AuthProvider { get; set; } = "Wechat";

        //[Required]
        //[StringLength(UserLogin.MaxProviderKeyLength)]
        //public string ProviderKey { get; set; }

        [Required]
        public string ProviderAccessCode { get; set; }
    }
}
