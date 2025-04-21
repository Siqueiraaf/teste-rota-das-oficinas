using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for the SaleItem entity
/// </summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasKey(si => si.Id);

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(si => si.Subtotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Configure relationship with Sale (parent)
        builder.HasOne(si => si.Sale)
               .WithMany(s => s.Items)
               .HasForeignKey(si => si.SaleId)
               .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with Product
        builder.HasOne(si => si.Product)
               .WithMany()
               .HasForeignKey(si => si.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
