using Com.DanLiris.Service.Auth.Lib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Com.DanLiris.Service.Auth.Lib.Configs
{
    class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(p => p.Code).HasMaxLength(255);
            builder.Property(p => p.Name).HasMaxLength(255);
            builder.Property(p => p.Description).HasMaxLength(3000);

            builder
                .HasMany(p => p.Permissions)
                .WithOne(p => p.Role)
                .HasForeignKey(p => p.RoleId);

            builder
                .HasMany(p => p.AccountRoles)
                .WithOne(p => p.Role)
                .HasForeignKey(p => p.RoleId);
        }
    }
}
