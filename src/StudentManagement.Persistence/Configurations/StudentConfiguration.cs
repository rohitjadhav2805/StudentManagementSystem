using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.Email)
            .IsUnique()
            .HasDatabaseName("IX_Students_Email");

        builder.HasIndex(s => s.Name)
            .HasDatabaseName("IX_Students_Name");

        builder.Property(s => s.Age)
            .IsRequired();

        builder.Property(s => s.Course)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
