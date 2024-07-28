using Datamarka_DomainModel.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Datamarka_DAL.DbEntityConfigurations.Identity
{
    internal class UserSettingsDBConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            builder.HasOne<User>()
                .WithOne(user => user.UserSettings)
                .HasForeignKey<UserSettings>(us => us.UserId);
        }
    }
}
