using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenderManagement.Domain.Entity;

namespace TenderManagement.Infrastructure.Persistence.Configuration
{
    public class TenderConfiguration : IEntityTypeConfiguration<Tender>
    {
        public void Configure(EntityTypeBuilder<Tender> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.RefNumber).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Details).IsRequired();
            builder.Property(p => p.CreatedBy).IsRequired().HasMaxLength(50);
            builder.HasQueryFilter(p => !p.IsDeleted);
            builder.Ignore(p => p.DomainEvents);
        }
    }
}