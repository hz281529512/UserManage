using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using UserManage.Authorization.Users;
using UserManage.Users.Dto;

namespace UserManage.SynchronizeCore.Dtos
{
    [AutoMapTo(typeof(User))]
    public class CreateAuthUserDto : CreateUserDto
    {

        /// <summary>
        /// 外部凭证(通常指Userid)
        /// </summary>
        public string AuthenticationVoucher { get; set; }

        /// <summary>
        /// 外部来源
        /// </summary>
        public string AuthenticationSource { get; set; }

    }
}
