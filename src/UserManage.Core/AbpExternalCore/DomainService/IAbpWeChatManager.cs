using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManage.AbpExternalCore.Model;

namespace UserManage.AbpExternalCore.DomainService
{
    public interface IAbpWeChatManager : IDomainService
    {

        /// <summary>
        /// 获取所有部门
        /// </summary>
        Task<ICollection<AbpWeChatDepartment>> GetAllDepartments();

        /// <summary>
        /// 根据父节点ID获取Department
        /// </summary>
        /// <param name="parent_id">父节点ID,根节点为1</param>
        Task<ICollection<AbpWeChatDepartment>> GetDepartmentsById(int? parent_id = null, bool is_only_one_layer = false);

        /// <summary>
        /// 根据Dept ID 获取所有Users
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        Task<ICollection<AbpWeChatUser>> GetAllUsersByDepartment(int? parent_id);

        /// <summary>
        /// 根据Dept ID 获取所有用户部门对照数据
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        Task<ICollection<Tuple<string, int>>> GetAllUsersDeptRelation();

        /// <summary>
        /// 根据Dept ID 获取所有Users
        /// </summary>
        /// <param name="wechat_tag_id"></param>
        /// <returns></returns>
        Task<ICollection<AbpWeChatUser>> GetUserListByTag(int? wechat_tag_id);

        /// <summary>
        /// 获取企业微信所有标签
        /// </summary>
        Task<ICollection<AbpWeChatTag>> GetAllTag();

        
        /// <summary>
        /// 根据Tag ID 获取所有标签名
        /// </summary>
        /// <param name="wechat_tag_id"></param>
        /// <param name="tenant_id"></param>
        /// <returns></returns>
        string GetTagNameById(int wechat_tag_id, int tenant_id);

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        Task<AbpUserSingleResult> GetUserById(string user_id);
    }
}
