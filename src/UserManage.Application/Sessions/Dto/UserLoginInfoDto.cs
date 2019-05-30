using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using UserManage.Authorization.Users;

namespace UserManage.Sessions.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }

        public string SelectDistrict { get; set; }

        public bool IsActive { get; set; }

        public string FullName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 企业微信头像Url
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>

        public string CompanyId { get; set; }

    }
}
