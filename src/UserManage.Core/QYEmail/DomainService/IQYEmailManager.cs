using System;
using System.Collections.Generic;
using System.Text;

namespace UserManage.QYEmail.DomainService
{
    public interface IQYEmailManager
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="tenant_id"></param>
        /// <returns></returns>
        List<QYMailDepartment> GetEmailAllDepartment(int tenant_id);

        QYMailUserInfocsForSeach GetUserInfo(int tenant_id, string email);

        void RemoveQYEmail(QYMailUserInfocsForUpdate model, int tenant_id);
    }
}
