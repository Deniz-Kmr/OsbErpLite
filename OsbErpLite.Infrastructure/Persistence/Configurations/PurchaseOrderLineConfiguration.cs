using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Infrastructure.Persistence.Configurations;

public class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.HasKey(pol => pol.Id);

        builder.Property(pol => pol.UnitPrice)
            .HasPrecision(18, 2);

        builder.HasOne(pol => pol.PurchaseOrder)
            .WithMany(po => po.Lines)
            .HasForeignKey(pol => pol.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pol => pol.Product)
            .WithMany(p => p.PurchaseOrderLines)
            .HasForeignKey(pol => pol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
