using FluentAssertions;
using InterviewEasy.BuildingBlocks.Common.Results;
using Xunit;

namespace InterviewEasy.BuildingBlocks.Common.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldSetIsSuccessTrue()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldSetIsSuccessFalse()
    {
        var error = Error.Validation("invalid", "Something was invalid");
        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Success_WithValue_ShouldReturnValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_AccessingValue_ShouldThrow()
    {
        var result = Result.Failure<int>(Error.NotFound("x", "Not found"));

        Action act = () => _ = result.Value;

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*failed result*");
    }
}
