

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
        /// WxId
        /// </summary>
        public string WxId { get; set; }



		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }



		/// <summary>
		/// ParentId
		/// </summary>
		public int? ParentId { get; set; }



		/// <summary>
		/// OrgGuid
		/// </summary>
		public string OrgGuid { get; set; }



		/// <summary>
		/// OrgParentGuid
		/// </summary>
		public string OrgParentGuid { get; set; }



		/// <summary>
		/// OrgOrderNo
		/// </summary>
		public int? OrgOrderNo { get; set; }




    }
}