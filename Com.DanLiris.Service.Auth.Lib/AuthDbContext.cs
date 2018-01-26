using Com.DanLiris.Service.Auth.Lib.Configs;
using Com.DanLiris.Service.Auth.Lib.Models;
using Com.Moonlay.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Com.DanLiris.Service.Auth.Lib
{
    public class AuthDbContext : BaseDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountProfile> AccountProfiles { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration<Account>(new AccountConfig());
            modelBuilder.ApplyConfiguration<AccountProfile>(new AccountProfileConfig());
            modelBuilder.ApplyConfiguration<Permission>(new PermissionConfig());
            modelBuilder.ApplyConfiguration<Role>(new RoleConfig());
        }
    }
}
