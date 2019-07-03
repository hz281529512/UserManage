

using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore.Dtos
{
    public class BaseUserRoleListDto 
    {
        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// ����Ͳ��ɫ�༭
        /// </summary>
        public bool? AllowLowEdit { get; set; }

    }
}