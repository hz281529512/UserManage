

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManage.BaseEntityCore;

namespace UserManage.EntityMapper.BaseUserOrgs
{
    public class BaseUserOrgCfg : IEntityTypeConfiguration<BaseUserOrg>
    {
        public void Configure(EntityTypeBuilder<BaseUserOrg> builder)
        {

            builder.ToTable("BaseUserOrgs", YoYoAbpefCoreConsts.SchemaNames.CMS);

            
			builder.Property(a => a.WxId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.Name).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.ParentId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.OrgGuid).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.OrgParentGuid).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.OrgOrderNo).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);


        }
    }
}


