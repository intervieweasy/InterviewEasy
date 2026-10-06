using FluentAssertions;
using InterviewEasy.BuildingBlocks.EventBus.Abstractions;
using Xunit;

namespace InterviewEasy.BuildingBlocks.EventBus.UnitTests;

public class IntegrationEventTests
{
    private sealed record TestEvent : IntegrationEvent;

    [Fact]
    public void NewEvent_ShouldHaveNonEmptyId()
    {
        var e = new TestEvent();

        e.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void NewEvent_ShouldDefaultToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var e = new TestEvent();
        var after = DateTime.UtcNow.AddSeconds(1);

        e.OccurredOn.Should().BeAfter(before).And.BeBefore(after);
    }

    [Fact]
    public void TwoEvents_ShouldHaveDifferentIds()
    {
        var a = new TestEvent();
        var b = new TestEvent();

        a.Id.Should().NotBe(b.Id);
    }
}
