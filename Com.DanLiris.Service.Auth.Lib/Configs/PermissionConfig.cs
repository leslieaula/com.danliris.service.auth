using Com.DanLiris.Service.Auth.Lib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Auth.Lib.Configs
{
    public class PermissionConfig : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.Property(p => p.Unit).HasMaxLength(255);
            builder.Property(p => p.UnitCode).HasMaxLength(255);
            builder.Property(p => p.Division).HasMaxLength(255);
        }
    }
}
