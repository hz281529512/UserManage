

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManage.ThirdPartyConfigCore;

namespace UserManage.EntityMapper.ThirdPartyConfigs
{
    public class ThirdPartyConfigCfg : IEntityTypeConfiguration<ThirdPartyConfig>
    {
        public void Configure(EntityTypeBuilder<ThirdPartyConfig> builder)
        {

            builder.ToTable("ThirdPartyConfigs", YoYoAbpefCoreConsts.SchemaNames.CMS);

            
			builder.Property(a => a.AppName).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.LoginProvider).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.SuiteID).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.Secret).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.Token).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.EncodingAESKey).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);
			builder.Property(a => a.IsEnabled).HasMaxLength(YoYoAbpefCoreConsts.EntityLengthNames.Length64);


        }
    }
}


