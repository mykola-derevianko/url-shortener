using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLShortener.Domain.Entities;

namespace URLShortener.Infrastructure.Data.Configurations
{
    public class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
    {
        public void Configure(EntityTypeBuilder<ShortUrl> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.OriginalUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(s => s.ShortCode)
                .IsRequired()
                .HasMaxLength(32);

            builder.HasIndex(s => s.ShortCode)
                .IsUnique();

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); //May be changed to Cascade, but idk if we need to delete urls if user is deleted
        }
    }
}
