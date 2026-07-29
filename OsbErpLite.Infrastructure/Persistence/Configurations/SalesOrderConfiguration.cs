using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Infrastructure.Persistence.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(so => so.Id);

        builder.Property(so => so.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(so => so.Customer)
            .WithMany(bp => bp.SalesOrders)
            .HasForeignKey(so => so.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.ApprovedByEmployee)
            .WithMany()
            .HasForeignKey(so => so.ApprovedByEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
