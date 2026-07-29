using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(po => po.Id);

        builder.Property(po => po.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(po => po.Supplier)
            .WithMany(bp => bp.PurchaseOrders)
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(po => po.ApprovedByEmployee)
            .WithMany()
            .HasForeignKey(po => po.ApprovedByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
