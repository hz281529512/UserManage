

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManage.BaseEntityCore;

namespace UserManage.EntityMapper.BaseUserRoles
{
    public class BaseUserRoleCfg : IEntityTypeConfiguration<BaseUserRole>
    {
        public void Configure(EntityTypeBuilder<BaseUserRole> builder)
        {

            builder.ToTable("BaseUserRoles", YoYoAbpefCoreConsts.SchemaNames.CMS);

            
			builder.Property(a => a.Name).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);


        }
    }
}


