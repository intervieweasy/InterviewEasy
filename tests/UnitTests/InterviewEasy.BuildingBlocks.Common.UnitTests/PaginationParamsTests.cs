using FluentAssertions;
using InterviewEasy.BuildingBlocks.Common.Pagination;
using Xunit;

namespace InterviewEasy.BuildingBlocks.Common.UnitTests;

public class PaginationParamsTests
{
    [Fact]
    public void Defaults_ShouldBe1And20()
    {
        var p = new PaginationParams();

        p.PageNumber.Should().Be(1);
        p.PageSize.Should().Be(20);
    }

    [Fact]
    public void NegativePageNumber_ShouldClampTo1()
    {
        var p = new PaginationParams { PageNumber = -5 };

        p.PageNumber.Should().Be(1);
    }

    [Fact]
    public void PageSizeOver100_ShouldClampTo100()
    {
        var p = new PaginationParams { PageSize = 500 };

        p.PageSize.Should().Be(100);
    }

    [Fact]
    public void Skip_ShouldBeComputedCorrectly()
    {
        var p = new PaginationParams { PageNumber = 3, PageSize = 10 };

        p.Skip.Should().Be(20);
    }
}
