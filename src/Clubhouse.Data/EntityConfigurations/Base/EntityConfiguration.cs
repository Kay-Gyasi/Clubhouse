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
        builder.HasKey(o => o.Id)
            .IsClustered(false);

        builder.Property(x => x.RowId)
            .UseIdentityColumn(1, 1);

        builder.HasIndex(x => x.RowId)
            .HasDatabaseName("row_idx")
            .IsClustered()
            .IsUnique();

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        
        builder.Property(c => c.CreatedBy)
            .IsRequired()
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
