using Clubhouse.Data.Entities;
using Clubhouse.Data.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clubhouse.Data.EntityConfigurations
{
    public class UserConfiguration : EntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.HasIndex(x => x.PhoneNumber)
                .HasDatabaseName("user_phone_idx");
            builder.HasOne(x => x.Bill)
                .WithOne(x => x.User)
                .HasForeignKey<Bill>(x => x.UserId);
        }
    }
}
