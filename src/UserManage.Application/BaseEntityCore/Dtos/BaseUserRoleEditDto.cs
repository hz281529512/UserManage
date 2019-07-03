
using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using UserManage.BaseEntityCore;

namespace  UserManage.BaseEntityCore.Dtos
{
    public class BaseUserRoleEditDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }



        /// <summary>
        /// Ãû³Æ
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// ÔÊÐíµÍ²ã½ÇÉ«±à¼­
        /// </summary>
        public bool? AllowLowEdit { get; set; }
    }
}