

using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore.Dtos
{
    public class BaseUserEmpRoleListDto 
    {

        public int? Id { get; set; }

        /// <summary>
        /// BaseUniqueId
        /// </summary>
        public string BaseUniqueId { get; set; }



		/// <summary>
		/// AbpUserId
		/// </summary>
		public string AbpUserId { get; set; }



		/// <summary>
		/// EmpUserId
		/// </summary>
		public string EmpUserId { get; set; }



		/// <summary>
		/// EmpUserGuid
		/// </summary>
		public string EmpUserGuid { get; set; }



		/// <summary>
		/// BaseRoleId
		/// </summary>
		public int BaseRoleId { get; set; }




    }
}