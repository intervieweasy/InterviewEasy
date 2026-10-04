using FluentAssertions;
using InterviewEasy.BuildingBlocks.Core.Entities;

namespace InterviewEasy.BuildingBlocks.Core.UnitTests;

public class AuditableEntityTests
{
    private sealed class TestAuditable : AuditableEntity
    {
        public void TestTouch(Guid? actor = null) => Touch(actor);
    }

    [Fact]
    public void NewAuditableEntity_SetsCreatedAndUpdatedAt()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var entity = new TestAuditable();
        var after = DateTime.UtcNow.AddSeconds(1);

        entity.CreatedAt.Should().BeAfter(before).And.BeBefore(after);
        entity.UpdatedAt.Should().BeAfter(before).And.BeBefore(after);
    }

    [Fact]
    public void Touch_UpdatesUpdatedAt_And_SetsUpdatedBy()
    {
        var entity = new TestAuditable();
        var original = entity.UpdatedAt;
        var actor = Guid.NewGuid();

        Thread.Sleep(10);
        entity.TestTouch(actor);

        entity.UpdatedAt.Should().BeAfter(original);
        entity.UpdatedBy.Should().Be(actor);
    }

    [Fact]
    public void Touch_WithoutActor_DoesNotSetUpdatedBy()
    {
        var entity = new TestAuditable();

        entity.TestTouch();

        entity.UpdatedBy.Should().BeNull();
    }
}