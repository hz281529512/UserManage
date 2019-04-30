using Abp.AutoMapper;
using System;
using System.Xml.Serialization;
using UserManage.AbpExternalCore;
using UserManage.SynchronizeCore;

namespace UserManage.QyCallBack.Model
{
    [XmlRoot("xml")]
    public class QyMsgBase
    {
        //企业微信CorpID
        public string ToUserName { get; set; }
        //此事件该值固定为sys，表示该消息由系统生成
        public string FromUserName { get; set; }
        //消息创建时间 （整型）
        public string CreateTime { get; set; }
        //消息的类型，此时固定为event
        public string MsgType { get; set; }
        //事件的类型，此时固定为change_contact
        public string Event { get; set; }
        //类型
        public string ChangeType { get; set; }
    }
    /// <summary>
    /// 用户
    /// </summary>
    [XmlRoot("xml")]
    [AutoMapFrom(typeof(AbpWeChatUser))]
    public class QyUserBase : QyMsgBase
    {
        //成员UserID
        public string UserID { get; set; }
        //成员名称
        public string Name { get; set; }
        //成员部门列表
        public string Department { get; set; }
        //表示所在部门是否为上级
        public string IsLeaderInDept { get; set; }
        //手机号码
        public string Mobile { get; set; }
        //	职位信息
        public string Position { get; set; }
        //性别
        public string Gender { get; set; }
        //邮箱
        public string Email { get; set; }
        //激活状态：1=已激活 2=已禁用 4=未激活 已激活代表已激活企业微信或已关注微工作台
        public string Status { get; set; }
        //头像url
        public string Avatar { get; set; }
        //成员别名
        public string Alias { get; set; }
        //	座机
        public string Telephone { get; set; }
        //地址
        public string Address { get; set; }
    }
    /// <summary>
    /// 部门
    /// </summary>
    [XmlRoot("xml")]
    [AutoMapFrom(typeof(SyncDepartment))]
    public class QyPartyBase : QyMsgBase
    {
        //部门Id
        public string Id { get; set; }
        //部门名称
        public string Name { get; set; }
        //父部门id
        public string ParentId { get; set; }
        //	部门排序
        public string Order { get; set; }
    }

    [XmlRoot("xml")]
    public class QyTagBase : QyMsgBase
    {
        //标签Id
        public string TagId { get; set; }
        //标签中新增的成员userid列表，用逗号分隔
        public string AddUserItems { get; set; }
        //标签中删除的成员userid列表，用逗号分隔
        public string DelUserItems { get; set; }
        //标签中新增的部门id列表，用逗号分隔
        public string AddPartyItems { get; set; }
        //标签中删除的部门id列表，用逗号分隔
        public string DelPartyItems { get; set; }
    }
}
