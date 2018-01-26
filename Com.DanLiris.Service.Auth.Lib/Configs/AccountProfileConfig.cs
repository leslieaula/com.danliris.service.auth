using Com.DanLiris.Service.Auth.Lib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.DanLiris.Service.Auth.Lib.Configs
{
    public class AccountProfileConfig : IEntityTypeConfiguration<AccountProfile>
    {
        public void Configure(EntityTypeBuilder<AccountProfile> builder)
        {
            builder.Property(p => p.Firstname).HasMaxLength(255);
            builder.Property(p => p.Lastname).HasMaxLength(255);
            builder.Property(p => p.Gender).HasMaxLength(6);
        }
    }
}
