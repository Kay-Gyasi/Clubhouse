using Clubhouse.Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clubhouse.Data.EntityConfigurations.Base;

public class EntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : Entity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        
        builder.Property(c => c.CreatedBy)
            .IsRequired()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
