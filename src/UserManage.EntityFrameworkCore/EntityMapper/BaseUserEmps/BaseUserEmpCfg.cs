

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManage.BaseEntityCore;

namespace UserManage.EntityMapper.BaseUserEmps
{
    public class BaseUserEmpCfg : IEntityTypeConfiguration<BaseUserEmp>
    {
        public void Configure(EntityTypeBuilder<BaseUserEmp> builder)
        {

            builder.ToTable("BaseUserEmps", YoYoAbpefCoreConsts.SchemaNames.CMS);

            
			builder.Property(a => a.AbpUserId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.EmpUserId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.EmpUserGuid).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.IsLeader).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.EmpStationId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.EmpOrderNo).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.EmpStatus).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);


        }
    }
}


