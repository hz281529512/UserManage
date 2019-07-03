

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManage.BaseEntityCore;

namespace UserManage.EntityMapper.BaseUserEmpRoles
{
    public class BaseUserEmpRoleCfg : IEntityTypeConfiguration<BaseUserEmpRole>
    {
        public void Configure(EntityTypeBuilder<BaseUserEmpRole> builder)
        {

            builder.ToTable("BaseUserEmpRoles", YoYoAbpefCoreConsts.SchemaNames.CMS);

            
			builder.Property(a => a.BaseUniqueId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.AbpUserId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.EmpUserId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.EmpUserGuid).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.BaseRoleId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);


        }
    }
}


