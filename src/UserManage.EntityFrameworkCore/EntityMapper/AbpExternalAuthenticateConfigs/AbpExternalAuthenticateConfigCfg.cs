

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManage.AbpExternalAuthenticateCore;

namespace UserManage.EntityMapper.AbpExternalAuthenticateConfigs
{
    public class AbpExternalAuthenticateConfigCfg : IEntityTypeConfiguration<AbpExternalAuthenticateConfig>
    {
        public void Configure(EntityTypeBuilder<AbpExternalAuthenticateConfig> builder)
        {

            builder.ToTable("AbpExternalAuthenticateConfigs", YoYoAbpefCoreConsts.SchemaNames.CMS);

            
			builder.Property(a => a.TenantId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.AppName).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.LoginProvider).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.AppId).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.Secret).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.IsEnabled).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);


        }
    }
}


