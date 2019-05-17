namespace UserManage.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";
        public const string Operations = "Operations";

        #region 系统管理
        public const string Pages_Manage = "Pages.Manage";//管理
        public const string Pages_Manage_Tenants = "Pages.Manage.Tenants";//租户
        public const string Pages_Manage_Users = "Pages.Manage.Users";//用户
        public const string Pages_Manage_UserInfo = "Pages.Manage.UserInfo";//用户信息
        public const string Pages_Manage_Roles = "Pages.Manage.Roles";//角色
        public const string Pages_Manage_OrganizationUnits = "Pages.Manage.OrganizationUnits";//组织
        public const string Pages_Manage_LeaveOffice = "Pages.Manage.LeaveOffice";//离职
        public const string Pages_Manage_Sync = "Pages.Manage.Sync";//同步
        public const string Pages_Manage_RoleAllot = "Pages.Manage.RoleAllot";//角色分配
        public const string Pages_Manage_Menus = "Pages.Manage.Menus";//菜单
        #endregion

        #region 个人中心
        public const string Pages_MyCenter = "Pages.MyCenter";//个人中心
        public const string Pages_MyCenter_Attendance = "Pages.MyCenter.Attendance";//个人考勤记录
        public const string Pages_MyCenter_QiyeMail = "Pages.MyCenter.QiyeMail";//企业邮箱密码
        public const string Pages_MyCenter_EditOA = "Pages.MyCenter.EditOA";//企业邮箱密码
        public const string Pages_MyCenter_ProjectShow = "Pages.MyCenter.ProjectShow";//项目一览
        public const string Pages_MyCenter_FinanceShow = "Pages.MyCenter.FinanceShow";//财务一览
        #endregion

        #region 提成申报
        public const string Pages_Royalty = "Pages.Royalty";
        public const string Pages_Royalty_Rule = "Pages.Royalty.Rule"; //提成规则
        public const string Pages_Royalty_Declare = "Pages.Royalty.Declare"; //提成项目申报
        public const string Pages_Royalty_Special = "Pages.Royalty.Special";//特殊项目申报
        public const string Pages_Royalty_Grant = "Pages.Royalty.Grant"; //提成发放申报
        public const string Pages_Royalty_RoyaltyHistory = "Pages.Royalty.RoyaltyHistory"; //过往提成一览
        #endregion

        #region 财务管控
        public const string Pages_Business = "Pages.Business";//财务管控
        //public const string Pages_Business_Articles = "Pages.Business.Articles";//文章公告
        public const string Pages_Business_Orders = "Pages.Business.Orders";//订单
        public const string Pages_Business_OrdersInvoice = "Pages.Business.OrdersInvoice";//发票
        public const string Pages_Business_ServiceCharge = "Pages.Business.ServiceCharge";//服务费
        public const string Pages_Business_Margin = "Pages.Business.Margin";//保证金
        public const string Pages_Business_RefundMargin = "Pages.Business.RefundMargin";//退保证金
        public const string Pages_Business_BidNotice = "Pages.Business.BidNotice";//中标通知书审核
        #endregion

        #region 客户管理及立项
        public const string Pages_ProjectManage = "Pages.ProjectManage";//项目管理及立项
        public const string Pages_ProjectManage_NewProject = "Pages.ProjectManage.NewProject";//项目立项
        public const string Pages_ProjectManage_BidManage = "Pages.ProjectManage.BidManage";//投标管理
        public const string Pages_ProjectManage_TrainingManage = "Pages.ProjectManage.TrainingManage";//培训管理
        public const string Pages_ProjectManage_CustomerFile = "Pages.ProjectManage.CustomerFile";//客户档案
        #endregion

        #region 文件编制
        public const string Pages_FileEdit = "Pages.FileEdit";//文件编制及挂网
        public const string Pages_FileEdit_ProjectConfirm = "Pages.FileEdit.ProjectConfirm";//项目立项确认
        public const string Pages_FileEdit_Net = "Pages.FileEdit.Net";//文件编制及挂网
        public const string Pages_FileEdit_FileAudit = "Pages.FileEdit.FileAudit";//文件编制及挂网
        #endregion

        #region 项目管理
        public const string Pages_Project = "Pages.Project";//项目列表
        public const string Pages_Project_WinBid = "Pages.Project.WinBid";//中标确认及打印
        public const string Pages_Project_Cancel = "Pages.Project.Cancel";//项目取消
        public const string Pages_Project_History = "Pages.Project.History";//项目历史
        public const string Pages_Project_FileTatol = "Pages.Project.FileTatol";//文件汇总
        public const string Pages_Project_ProjectEdit = "Pages.Project.ProjectEdit";//项目编辑菜单
        public const string Pages_Project_ProjectNotice = "Pages.Project.ProjectNotice";//项目公告审查
        #endregion

        //#region 项目经理

        //public const string Pages_ProjectManager = "Pages.ProjectManager";//项目经理菜单


        //#endregion

        #region 电子评标及会务
        public const string Pages_AM = "Pages.AM";//会务管理
        public const string Pages_AM_EBid = "Pages.AM.EBid";//电子评标
        public const string Pages_AM_Calendar = "Pages.AM.Calendar";//会务管理
        public const string Pages_AM_Meeting = "Pages.AM.Meeting";//会务管理
        #endregion

        #region 公告
        public const string Pages_Articles = "Pages.Articles";//公告
        public const string Pages_Articles_Issue = "Pages.Articles.Issue";//招标公告
        public const string Pages_Articles_Corr = "Pages.Articles.Corr";//更正公告
        public const string Pages_Articles_Result = "Pages.Articles.Result";//更正公告
        public const string Pages_Articles_Other = "Pages.Articles.Other";//其他公告
        public const string Pages_Articles_MyApply = "Pages.Articles.MyApply";//我的报名
        #endregion

        #region 交易平台管理
        public const string Pages_Trading = "Pages.Trading";//交易平台
        public const string Pages_Trading_Lead = "Pages.Trading.Lead";//电子竞价项目导入
        public const string Pages_Trading_OldPlatform = "Pages.Trading.OldPlatform";//旧平台
        public const string Pages_Trading_NewPlatform = "Pages.Trading.NewPlatform";//新平台
        public const string Pages_Trading_SExplain = "Pages.Trading.SExplain";//评标报告更换供应商说明
        #endregion

        #region 专家管理

        public const string Pages_Expert = "Pages.Expert";//专家管理
        public const string Pages_Expert_Library = "Pages.Expert.Library";//专家库
        public const string Pages_Expert_Extract = "Pages.Expert.Extract";//专家抽取
        public const string Pages_Expert_Examine = "Pages.Expert.Examine";//专家审核
        #endregion






        #region 质控中心
        public const string Pages_ZK = "Pages.ZK";//质控中心
        #endregion

        #region 人力模块
        public const string Pages_HR = "Pages.HR";//人力
        #endregion

        #region 行政模块
        public const string Pages_ADM = "Pages.ADM";//行政管理
        #endregion

        #region 业务系统后台管理
        public const string Pages_Report = "Pages.Report";//报表

        #endregion

        #region 财务模块
        public const string Pages_FD = "Pages.FD";//财务
        #endregion

        #region 汇总报表
        public const string Pages_Gather = "Pages.Gather";//汇总
        #endregion

        #region 工作流模块
        public const string Pages_Workflow = "Pages.Workflow";//工作流模块
        //public const string Pages_Workflow_Project = "Pages.workflow.Project";//项目立项
        #endregion
        //public const string Pages_Nproject = "Pages.Nproject";//项目

        #region 集点数据
        public const string Pages_Point = "Pages.Point";
        #endregion

        #region 操作按钮
        public const string Operations_NetButton = "Operations.NetButton";
        public const string Operations_InvoiceButton = "Operations.InvoiceButton";

        #endregion
    }
}
