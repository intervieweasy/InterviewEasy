using InterviewEasy.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewEasy.Identity.Infrastructure.Persistence.Configurations;

public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");
        builder.HasKey(prt => prt.Id);

        builder.Property(prt => prt.Id).HasColumnName("id");
        builder.Property(prt => prt.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(prt => prt.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(prt => prt.TokenHash).HasColumnName("token_hash").HasMaxLength(500).IsRequired();
        builder.Property(prt => prt.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(prt => prt.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(prt => prt.UsedAt).HasColumnName("used_at");
        builder.Property(prt => prt.RequestIp).HasColumnName("request_ip");
        builder.Property(prt => prt.RequestUserAgent).HasColumnName("request_user_agent");

        builder.HasIndex(prt => prt.TokenHash).IsUnique().HasDatabaseName("uq_password_reset_tokens_hash");
        builder.HasIndex(prt => prt.UserId).HasDatabaseName("idx_password_reset_tokens_user");
    }
}
