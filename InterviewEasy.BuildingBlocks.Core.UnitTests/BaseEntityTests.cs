using InterviewEasy.BuildingBlocks.Core.Entities;

namespace InterviewEasy.BuildingBlocks.Core.UnitTests;

public class BaseEntityTests
{
    private sealed class TestEntity : BaseEntity { }

    [Fact]
    public void NewEntity_HasNonEmptyId()
    {
        var entity = new TestEntity();

        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void TwoEntities_WithSameId_AreEqual()
    {
        var id = Guid.NewGuid();
        var a = new TestEntity();
        var b = new TestEntity();

        typeof(BaseEntity)
            .GetProperty("Id")!
            .SetValue(a, id);
        typeof(BaseEntity)
            .GetProperty("Id")!
            .SetValue(b, id);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void TwoEntities_WithDifferentIds_AreNotEqual()
    {
        var a = new TestEntity();
        var b = new TestEntity();

        a.Should().NotBe(b);
        (a != b).Should().BeTrue();
    }
}