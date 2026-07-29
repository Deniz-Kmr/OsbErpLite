using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Infrastructure.Persistence.Configurations;

public class SalesOrderLineConfiguration : IEntityTypeConfiguration<SalesOrderLine>
{
    public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        builder.HasKey(sol => sol.Id);

        builder.Property(sol => sol.UnitPrice)
            .HasPrecision(18, 2);

        builder.HasOne(sol => sol.SalesOrder)
            .WithMany(so => so.Lines)
            .HasForeignKey(sol => sol.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sol => sol.Product)
            .WithMany(p => p.SalesOrderLines)
            .HasForeignKey(sol => sol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
