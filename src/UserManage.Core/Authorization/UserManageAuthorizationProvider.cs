using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace UserManage.Authorization
{
    public class UserManageAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            #region 页面

            var pages = context.GetPermissionOrNull(PermissionNames.Pages) ??
                        context.CreatePermission(PermissionNames.Pages, L("Pages"));

            //管理模块

            #region 系统管理

            var manage = pages.CreateChildPermission(PermissionNames.Pages_Manage, L("Manage"));//系统管理
            manage.CreateChildPermission(PermissionNames.Pages_Manage_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);//租户
            manage.CreateChildPermission(PermissionNames.Pages_Manage_Users, L("Users"));//用户
            manage.CreateChildPermission(PermissionNames.Pages_Manage_Roles, L("Roles"));//角色
            manage.CreateChildPermission(PermissionNames.Pages_Manage_OrganizationUnits, L("OrganizationUnits"));//组织
            manage.CreateChildPermission(PermissionNames.Pages_Manage_LeaveOffice, L("LeaveOffice"));//离职
            manage.CreateChildPermission(PermissionNames.Pages_Manage_Sync, L("Sync"));//用户同步
            manage.CreateChildPermission(PermissionNames.Pages_Manage_RoleAllot, L("RoleAllot"));//角色分配
            manage.CreateChildPermission(PermissionNames.Pages_Manage_UserInfo, L("UserInfo"));//用户信息
            manage.CreateChildPermission(PermissionNames.Pages_Manage_Menus, L("Menus"));//菜单
            #endregion

            #region 个人中心
            var mycenter = pages.CreateChildPermission(PermissionNames.Pages_MyCenter, L("MyCenter"));
            mycenter.CreateChildPermission(PermissionNames.Pages_MyCenter_Attendance, L("Attendance"));//个人考勤记录
            mycenter.CreateChildPermission(PermissionNames.Pages_MyCenter_QiyeMail, L("QiyeMail"));//企业邮箱密码
            mycenter.CreateChildPermission(PermissionNames.Pages_MyCenter_EditOA, L("EditOA"));//企业邮箱密码
            mycenter.CreateChildPermission(PermissionNames.Pages_MyCenter_ProjectShow, L("ProjectShow"));//项目一览
            mycenter.CreateChildPermission(PermissionNames.Pages_MyCenter_FinanceShow, L("FinanceShow"));//财务一览

            #endregion

            #region 提成申报
            var royalty = pages.CreateChildPermission(PermissionNames.Pages_Royalty, L("Royalty"));
            royalty.CreateChildPermission(PermissionNames.Pages_Royalty_Rule, L("Rule"));//提成规则
            royalty.CreateChildPermission(PermissionNames.Pages_Royalty_Declare, L("Declare"));//提成项目申报
            royalty.CreateChildPermission(PermissionNames.Pages_Royalty_Special, L("Special"));//特殊项目申报
            royalty.CreateChildPermission(PermissionNames.Pages_Royalty_Grant, L("Grant"));//提成发放申报
            royalty.CreateChildPermission(PermissionNames.Pages_Royalty_RoyaltyHistory, L("RoyaltyHistory"));//过往提成一览
            #endregion 


            #region 财务管控
            var business = pages.CreateChildPermission(PermissionNames.Pages_Business, L("Business"));
            business.CreateChildPermission(PermissionNames.Pages_Business_Orders, L("Orders"));
            business.CreateChildPermission(PermissionNames.Pages_Business_OrdersInvoice, L("OrdersInvoice"));
            business.CreateChildPermission(PermissionNames.Pages_Business_ServiceCharge, L("ServiceCharge"));
            business.CreateChildPermission(PermissionNames.Pages_Business_Margin, L("Margin"));
            business.CreateChildPermission(PermissionNames.Pages_Business_RefundMargin, L("RefundMargin"));
            business.CreateChildPermission(PermissionNames.Pages_Business_BidNotice, L("BidNotice"));//中标通知书审核
            #endregion

            #region 客户管理及立项

            //< !--2019 - 01 - 08 项目管理更名为客户管理-- >
            var projectManage = pages.CreateChildPermission(PermissionNames.Pages_ProjectManage, L("ProjectManage"));
            projectManage.CreateChildPermission(PermissionNames.Pages_ProjectManage_NewProject, L("NewProject"));
            projectManage.CreateChildPermission(PermissionNames.Pages_ProjectManage_BidManage, L("BidManage"));
            projectManage.CreateChildPermission(PermissionNames.Pages_ProjectManage_TrainingManage, L("TrainingManage"));
            projectManage.CreateChildPermission(PermissionNames.Pages_ProjectManage_CustomerFile, L("CustomerFile"));
            #endregion

            #region 文件编制及挂网
            var fileedit = pages.CreateChildPermission(PermissionNames.Pages_FileEdit, L("FileEdit"));
            fileedit.CreateChildPermission(PermissionNames.Pages_FileEdit_ProjectConfirm, L("ProjectConfirm"));//项目立项确认
            fileedit.CreateChildPermission(PermissionNames.Pages_FileEdit_Net, L("Net"));//编制及挂网
            fileedit.CreateChildPermission(PermissionNames.Pages_FileEdit_FileAudit, L("FileAudit"));//文件审核
            #endregion

            #region 项目管理 

            var project = pages.CreateChildPermission(PermissionNames.Pages_Project, L("Project"));
            project.CreateChildPermission(PermissionNames.Pages_Project_WinBid, L("WinBid"));//中标确认及打印
            project.CreateChildPermission(PermissionNames.Pages_Project_Cancel, L("Cancel"));//项目取消
            project.CreateChildPermission(PermissionNames.Pages_Project_History, L("History"));//历史项目
            project.CreateChildPermission(PermissionNames.Pages_Project_FileTatol, L("FileTatol"));//文件汇总
            project.CreateChildPermission(PermissionNames.Pages_Project_ProjectEdit, L("ProjectEdit"));//项目编辑菜单
            project.CreateChildPermission(PermissionNames.Pages_Project_ProjectNotice, L("ProjectNotice"));//项目公告审查
            #endregion



            #region 电子评标及会务
            var am = pages.CreateChildPermission(PermissionNames.Pages_AM, L("AM"));
            am.CreateChildPermission(PermissionNames.Pages_AM_EBid, L("EBid"));//电子评标
            am.CreateChildPermission(PermissionNames.Pages_AM_Calendar, L("Calendar"));//开标日历表
            am.CreateChildPermission(PermissionNames.Pages_AM_Meeting, L("Meeting"));//开标日历表
            #endregion

            #region 公告
            var articles = pages.CreateChildPermission(PermissionNames.Pages_Articles, L("Articles"));
            articles.CreateChildPermission(PermissionNames.Pages_Articles_Issue, L("Issue"));//招标公告
            articles.CreateChildPermission(PermissionNames.Pages_Articles_Corr, L("Corr"));//更正公告
            articles.CreateChildPermission(PermissionNames.Pages_Articles_Result, L("Result"));//结果公告
            articles.CreateChildPermission(PermissionNames.Pages_Articles_Other, L("Other"));//其他公告
            articles.CreateChildPermission(PermissionNames.Pages_Articles_MyApply, L("MyApply"));//我的报名
            #endregion

            #region 交易平台管理工具
            var trading = pages.CreateChildPermission(PermissionNames.Pages_Trading, L("Trading"));
            trading.CreateChildPermission(PermissionNames.Pages_Trading_Lead, L("Lead"));//电子竞价项目导入
            trading.CreateChildPermission(PermissionNames.Pages_Trading_OldPlatform, L("OldPlatform"));//旧平台
            trading.CreateChildPermission(PermissionNames.Pages_Trading_NewPlatform, L("NewPlatform"));//新平台
            trading.CreateChildPermission(PermissionNames.Pages_Trading_SExplain, L("SExplain"));//评标报告更换供应商说明
            #endregion

            #region 专家管理

            var expert = pages.CreateChildPermission(PermissionNames.Pages_Expert, L("Expert"));//专家管理
            expert.CreateChildPermission(PermissionNames.Pages_Expert_Library, L("Library"));//专家库
            expert.CreateChildPermission(PermissionNames.Pages_Expert_Extract, L("Extract"));//专家抽取
            expert.CreateChildPermission(PermissionNames.Pages_Expert_Examine, L("Examine"));//专家审核


            #endregion

            #region 质控中心
            pages.CreateChildPermission(PermissionNames.Pages_ZK, L("ZK"));
            #endregion

            #region 行政模块
            pages.CreateChildPermission(PermissionNames.Pages_ADM, L("ADM"));
            #endregion

            #region 人力模块
            pages.CreateChildPermission(PermissionNames.Pages_HR, L("HR"));
            #endregion

            #region 业务系统后台管理
            pages.CreateChildPermission(PermissionNames.Pages_Report, L("Report"));
            #endregion

            #region 财务模块
            pages.CreateChildPermission(PermissionNames.Pages_FD, L("FD"));
            #endregion

            #region 汇总报表
            pages.CreateChildPermission(PermissionNames.Pages_Gather, L("Gather"));
            #endregion

            #region 工作流模块
            pages.CreateChildPermission(PermissionNames.Pages_Workflow, L("Workflow"));
            #endregion
            #region 集点数据

            pages.CreateChildPermission(PermissionNames.Pages_Point, L("Point"));


            #endregion


            #region 操作
            var operations = context.GetPermissionOrNull(PermissionNames.Operations) ??
                        context.CreatePermission(PermissionNames.Operations, L("Operations"));
            operations.CreateChildPermission(PermissionNames.Operations_NetButton, L("NetButton"));
            operations.CreateChildPermission(PermissionNames.Operations_InvoiceButton, L("InvoiceButton"));


            #endregion
            #endregion
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, UserManageConsts.LocalizationSourceName);
        }
    }
}
