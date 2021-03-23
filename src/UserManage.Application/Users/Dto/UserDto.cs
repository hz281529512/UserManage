using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using UserManage.Authorization.Users;

namespace UserManage.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        /// <summary>
        /// �û���
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// �ֻ�����
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// �Ƿ񼤻�
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// ȫ��
        /// </summary>
        public string FullName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// ��ҵ΢��ͷ��Url
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// ְ��
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// �Ա�
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// ��˾id
        /// </summary>

        public string CompanyId { get; set; }
        /// <summary>
        /// ��ѯ����
        /// </summary>
        public string SelectDistrict { get; set; }

        /// <summary>
        /// �ͻ��޶�
        /// </summary>
        public int? ClientQuota { get; set; }

        public string[] RoleNames { get; set; }
    }
}
