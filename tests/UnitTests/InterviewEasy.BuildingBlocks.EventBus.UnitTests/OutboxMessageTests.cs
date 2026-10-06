using FluentAssertions;
using InterviewEasy.BuildingBlocks.EventBus.Outbox;
using Xunit;

namespace InterviewEasy.BuildingBlocks.EventBus.UnitTests;

public class OutboxMessageTests
{
    [Fact]
    public void Create_WithValidInput_ShouldSucceed()
    {
        var msg = OutboxMessage.Create(
            "TestEvent",
            "{\"foo\":\"bar\"}",
            DateTime.UtcNow);

        msg.EventType.Should().Be("TestEvent");
        msg.EventData.Should().Be("{\"foo\":\"bar\"}");
        msg.PublishedAt.Should().BeNull();
        msg.RetryCount.Should().Be(0);
    }

    [Fact]
    public void Create_WithEmptyEventType_ShouldThrow()
    {
        Action act = () => OutboxMessage.Create("", "{}", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("eventType");
    }

    [Fact]
    public void MarkPublished_ShouldSetPublishedAt()
    {
        var msg = OutboxMessage.Create("TestEvent", "{}", DateTime.UtcNow);

        msg.MarkPublished();

        msg.PublishedAt.Should().NotBeNull();
        msg.LastError.Should().BeNull();
    }

    [Fact]
    public void RecordFailure_ShouldIncrementRetryCount()
    {
        var msg = OutboxMessage.Create("TestEvent", "{}", DateTime.UtcNow);

        msg.RecordFailure("Something failed");
        msg.RecordFailure("Failed again");

        msg.RetryCount.Should().Be(2);
        msg.LastError.Should().Be("Failed again");
    }
}
