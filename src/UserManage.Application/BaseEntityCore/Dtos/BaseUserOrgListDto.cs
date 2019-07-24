

using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore.Dtos
{
    public class BaseUserOrgListDto 
    {

        public int? Id { get; set; }


        /// <summary>
        /// 微信ID
        /// </summary>
        public string WxId { get; set; }



		/// <summary>
		/// 部门名称
		/// </summary>
		public string Name { get; set; }



		/// <summary>
		/// ParentId
		/// </summary>
		public int? ParentId { get; set; }



		/// <summary>
		/// 部门Guid
		/// </summary>
		public string OrgGuid { get; set; }



		/// <summary>
		/// 部门父级GUID
		/// </summary>
		public string OrgParentGuid { get; set; }



		/// <summary>
		/// OrgOrderNo
		/// </summary>
		public int? OrgOrderNo { get; set; }




    }
}