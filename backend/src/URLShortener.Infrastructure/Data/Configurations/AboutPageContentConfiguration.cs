using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLShortener.Domain.Entities;

namespace URLShortener.Infrastructure.Data.Configurations
{
    public class AboutPageContentConfiguration : IEntityTypeConfiguration<AboutPageContent>
    {
        public void Configure(EntityTypeBuilder<AboutPageContent> builder)
        {
            builder.Property(x => x.Content)
                .IsRequired();
        }
    }
}
