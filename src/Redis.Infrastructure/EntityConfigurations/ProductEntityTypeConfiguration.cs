using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Redis.Domain.AggregatesModel.ProductAggregate;

namespace Redis.Infrastructure.EntityConfigurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> productConfiguration)
    {
        productConfiguration.ToTable("styles", ProductContext.DEFAULT_SCHEMA);

        productConfiguration.HasKey(p => p.Id);
        
        productConfiguration.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        productConfiguration.Ignore(p => p.DomainEvents);

        productConfiguration.Property("_gender")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(50)
            .HasColumnName("Gender");

        productConfiguration.Property("_masterCategory")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(50)
            .HasColumnName("MasterCategory");

        productConfiguration.Property("_subCategory")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(50)
            .HasColumnName("SubCategory");
        
        productConfiguration.Property("_articleType")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(50)
            .HasColumnName("ArticleType");
        
        productConfiguration.Property("_baseColour")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(50)
            .HasColumnName("BaseColour");
        
        productConfiguration.Property("_season")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(50)
            .HasColumnName("Season");
        
        productConfiguration.Property<int>("_year")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Year");
        
        productConfiguration.Property("_usage")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasMaxLength(50)
            .HasColumnName("Usage");
        
        productConfiguration.Property("_productDisplayName")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("ProductDisplayName")
            .HasMaxLength(100)
            .IsRequired();
    }
}