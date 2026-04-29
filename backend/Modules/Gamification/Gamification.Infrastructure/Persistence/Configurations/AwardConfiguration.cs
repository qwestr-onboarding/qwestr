using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Modules.Gamification.Domain.ValueObjects;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Configurations;

public class AwardConfiguration : IEntityTypeConfiguration<Award>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public void Configure(EntityTypeBuilder<Award> builder)
    {
        builder.ToTable("awards", "gamification");

        builder.Property(a => a.Code).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Title).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(1000);
        builder.Property(a => a.IconUrl).HasMaxLength(500).IsRequired();
        builder.Property(a => a.IssuanceType).HasConversion<int>();

        builder.Property(a => a.Rule)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(new
                {
                    trigger = (int)v.Trigger,
                    condition = (int)v.Condition,
                    targetValue = v.TargetValue,
                    filterParams = v.FilterParams
                }, JsonOptions),
                v => v == null ? null : DeserializeRule(v))
            .HasColumnType("jsonb")
            .HasColumnName("rule");

        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => new { a.TenantId, a.IsActive });
    }

    private static AwardRule? DeserializeRule(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var trigger = (RuleTrigger)root.GetProperty("trigger").GetInt32();
        var condition = (RuleCondition)root.GetProperty("condition").GetInt32();
        var targetValue = root.GetProperty("targetValue").GetInt32();

        var filterParams = new Dictionary<string, string>();
        if (root.TryGetProperty("filterParams", out var fp))
        {
            foreach (var prop in fp.EnumerateObject())
            {
                filterParams[prop.Name] = prop.Value.GetString() ?? string.Empty;
            }
        }

        var result = AwardRule.Create(trigger, condition, targetValue, filterParams);
        return result.IsSuccess ? result.Value : null;
    }
}
