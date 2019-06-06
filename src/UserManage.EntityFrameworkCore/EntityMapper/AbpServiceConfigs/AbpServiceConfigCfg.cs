

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManage.AbpServiceCore;

namespace UserManage.EntityMapper.AbpServiceConfigs
{
    public class AbpServiceConfigCfg : IEntityTypeConfiguration<AbpServiceConfig>
    {
        public void Configure(EntityTypeBuilder<AbpServiceConfig> builder)
        {

            builder.ToTable("AbpServiceConfigs", YoYoAbpefCoreConsts.SchemaNames.CMS);

            
			builder.Property(a => a.LocalName).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.AppName).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.LoginProvider).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.CropId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.AppId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.Secret).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.IsEnabled).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);


        }
    }
}


