using Clubhouse.Data.Entities;
using Clubhouse.Data.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Clubhouse.Data.EntityConfigurations;

public class BillConfiguration : EntityConfiguration<Bill>
{
    public override void Configure(EntityTypeBuilder<Bill> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Status)
            .HasConversion(new EnumToStringConverter<BillStatus>());
    }
}