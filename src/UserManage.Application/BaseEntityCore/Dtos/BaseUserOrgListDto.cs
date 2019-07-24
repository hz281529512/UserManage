

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
        /// ΢��ID
        /// </summary>
        public string WxId { get; set; }



		/// <summary>
		/// ��������
		/// </summary>
		public string Name { get; set; }



		/// <summary>
		/// ParentId
		/// </summary>
		public int? ParentId { get; set; }



		/// <summary>
		/// ����Guid
		/// </summary>
		public string OrgGuid { get; set; }



		/// <summary>
		/// ���Ÿ���GUID
		/// </summary>
		public string OrgParentGuid { get; set; }



		/// <summary>
		/// OrgOrderNo
		/// </summary>
		public int? OrgOrderNo { get; set; }




    }
}