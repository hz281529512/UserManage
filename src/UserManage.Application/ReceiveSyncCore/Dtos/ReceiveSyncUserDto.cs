using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using UserManage.Authorization.Users;
using UserManage.Users.Dto;

namespace UserManage.ReceiveSyncCore.Dtos
{
    public class ReceiveSyncUserDto : ReceiveSyncDto
    {
        

        /// <summary>
        /// 企业微信ID
        /// </summary>
        [Required]
        public string WXId { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public ReceiveSyncCreateUserDto Data { get; set; }
    }

    [AutoMapTo(typeof(User))]
    public class ReceiveSyncCreateUserDto : IShouldNormalize
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        public string[] RoleNames { get; set; }


        /// <summary>
        /// 手机号码
        /// </summary>
        [Required]
        [Phone]
        [StringLength(AbpUserBase.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 查询地区
        /// </summary>
        public string SelectDistrict { get; set; }

        /// <summary>
        /// 企业微信头像Url
        /// </summary>
        public string Avatar { get; set; }

        public void Normalize()
        {
            if (RoleNames == null)
            {
                RoleNames = new string[0];
            }
        }
    }

}
