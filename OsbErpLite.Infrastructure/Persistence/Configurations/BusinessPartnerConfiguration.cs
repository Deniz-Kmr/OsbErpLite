using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Infrastructure.Persistence.Configurations;

public class BusinessPartnerConfiguration : IEntityTypeConfiguration<BusinessPartner>
{
    public void Configure(EntityTypeBuilder<BusinessPartner> builder)
    {
        builder.HasKey(bp => bp.Id);

        builder.Property(bp => bp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(bp => bp.TaxNumber)
            .HasMaxLength(20);

        builder.HasIndex(bp => bp.TaxNumber)
            .IsUnique()
            .HasFilter("\"TaxNumber\" IS NOT NULL");

        builder.Property(bp => bp.Email)
            .HasMaxLength(200);

        builder.Property(bp => bp.Phone)
            .HasMaxLength(30);

        builder.Property(bp => bp.Address)
            .HasMaxLength(500);

        builder.Property(bp => bp.City)
            .HasMaxLength(100);
    }
}
