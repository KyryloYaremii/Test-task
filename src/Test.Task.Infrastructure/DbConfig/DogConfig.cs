using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Task.Domain.Entities;

namespace Test.Task.Infrastructure.Config;

public class AddressConfig : IEntityTypeConfiguration<Dog>
{
    public void Configure(EntityTypeBuilder<Dog> builder)
    {
        builder.ToTable("Dogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Color).IsRequired().HasMaxLength(100);
        builder.Property(x => x.TailLength).IsRequired();
        builder.Property(x => x.Weight).IsRequired();
    }
}