using InterviewEasy.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewEasy.Identity.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id).HasColumnName("id");
        builder.Property(rt => rt.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(rt => rt.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(rt => rt.TokenHash).HasColumnName("token_hash").HasMaxLength(500).IsRequired();
        builder.Property(rt => rt.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(rt => rt.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(rt => rt.RevokedAt).HasColumnName("revoked_at");
        builder.Property(rt => rt.ReplacedBy).HasColumnName("replaced_by");
        builder.Property(rt => rt.UserAgent).HasColumnName("user_agent");
        builder.Property(rt => rt.IpAddress).HasColumnName("ip_address");
        builder.Property(rt => rt.LastUsedAt).HasColumnName("last_used_at");

        builder.HasIndex(rt => rt.TokenHash).IsUnique().HasDatabaseName("uq_refresh_tokens_hash");
        builder.HasIndex(rt => rt.UserId).HasDatabaseName("idx_refresh_tokens_user");
    }
}
