
using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Abp.UI;
using Abp.AutoMapper;
using Abp.Authorization;
using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using Abp.Application.Services;
using Abp.Application.Services.Dto;


using UserManage.BaseEntityCore.Dtos;
using UserManage.BaseEntityCore;

namespace UserManage.BaseEntityCore
{
    /// <summary>
    /// BaseUserOrg应用层服务的接口方法
    ///</summary>
    public interface IBaseUserOrgAppService : IApplicationService
    {
        

    }
}
