
using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using UserManage.BaseEntityCore;

namespace  UserManage.BaseEntityCore.Dtos
{
    public class BaseUserEmpRoleEditDto
    {

        /// <summary>
        /// Id
        /// </summary>
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