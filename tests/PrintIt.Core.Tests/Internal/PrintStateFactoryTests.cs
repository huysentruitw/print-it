using System;
using FluentAssertions;
using PrintIt.Core.Internal;
using PrintIt.Core.Internal.PageRangeString;
using Xunit;

namespace PrintIt.Core.Tests.Internal
{
    public sealed class PrintStateFactoryTests
    {
        [Fact]
        public void Create_WithoutPageRange_ShouldReturnStateThatPrintsAllPages()
        {
            // Act
            PrintState state = PrintStateFactory.Create(null, documentPageCount: 3);

            // Assert
            state.CurrentPageIndex.Should().Be(0);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(1);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(2);
            state.AdvanceToNextPage().Should().BeFalse();
        }

        [Theory]
        [InlineData("13")]
        [InlineData("13 ")]
        [InlineData(" 13")]
        [InlineData(" 13 ")]
        public void Create_SinglePage_ShouldReturnStateToPrintTheRequiredPage(string pageRange)
        {
            // Act
            PrintState state = PrintStateFactory.Create(null, documentPageCount: 15, pageRange);

            // Assert
            state.CurrentPageIndex.Should().Be(12);
            state.AdvanceToNextPage().Should().BeFalse();
        }

        [Theory]
        [InlineData("4,2")]
        [InlineData("4, 2")]
        [InlineData("4 ,2")]
        [InlineData("4 , 2")]
        [InlineData(" 4 , 2 ")]
        public void Create_WithTwoSeparatePagesInRange_ShouldReturnStateToPrintBothPages(string pageRange)
        {
            // Act
            PrintState state = PrintStateFactory.Create(null, documentPageCount: 10, pageRange);

            // Assert
            state.CurrentPageIndex.Should().Be(3);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(1);
            state.AdvanceToNextPage().Should().BeFalse();
        }

        [Theory]
        [InlineData("5-8")]
        [InlineData("5- 8")]
        [InlineData("5 - 8")]
        [InlineData(" 5 - 8 ")]
        public void Create_ClosedPageRange_ShouldReturnStateToPrintPagesInRange(string pageRange)
        {
            // Act
            PrintState state = PrintStateFactory.Create(null, documentPageCount: 10, pageRange);

            // Assert
            state.CurrentPageIndex.Should().Be(4);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(5);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(6);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(7);
            state.AdvanceToNextPage().Should().BeFalse();
        }

        [Theory]
        [InlineData("7-")]
        [InlineData("7 -")]
        [InlineData(" 7 - ")]
        public void Create_WithOpenRangeWithStartPage_ShouldReturnStateToPrintAllPagesStartingAtGivenPage(string pageRange)
        {
            // Act
            PrintState state = PrintStateFactory.Create(null, documentPageCount: 10, pageRange);

            // Assert
            state.CurrentPageIndex.Should().Be(6);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(7);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(8);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(9);
            state.AdvanceToNextPage().Should().BeFalse();
        }

        [Theory]
        [InlineData("-3")]
        [InlineData("- 3")]
        [InlineData(" - 3 ")]
        public void Create_WithOpenRangeWithEndPage_ShouldReturnStateToPrintAllPagesUpUntilTheGivenPage(string pageRange)
        {
            // Act
            PrintState state = PrintStateFactory.Create(null, documentPageCount: 10, pageRange);

            // Assert
            state.CurrentPageIndex.Should().Be(0);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(1);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(2);
            state.AdvanceToNextPage().Should().BeFalse();
        }

        [Fact]
        public void Create_WithSinglePagesCombinedWithRanges_ShouldReturnStateToPrintAllRequestedPages()
        {
            // Act
            PrintState state = PrintStateFactory.Create(null, documentPageCount: 10, "8, 3-5, 1, , -2, 9-");

            // Assert
            state.CurrentPageIndex.Should().Be(7);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(2);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(3);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(4);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(0);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(0);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(1);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(8);
            state.AdvanceToNextPage().Should().BeTrue();
            state.CurrentPageIndex.Should().Be(9);
            state.AdvanceToNextPage().Should().BeFalse();
        }

        [Theory]
        [InlineData("8,k", "Invalid character found in source: k")]
        [InlineData("8--8", "Expected separator or end token instead of -")]
        [InlineData("4 8", "Expected separator or end token instead of 8")]
        [InlineData("8-8,1-2-4", "Expected separator or end token instead of -")]
        [InlineData("abc", "Invalid character found in source: a")]
        public void Create_InvalidPageRangeString_ShouldThrowPageRangeFormatException(string pageString, string expectedMessage)
        {
            // Act
            Action action = () => PrintStateFactory.Create(null, documentPageCount: 10, pageString);

            // Assert
            action.Should().Throw<PageRangeStringFormatException>()
                .WithMessage(expectedMessage);
        }

        [Theory]
        [InlineData("0", "Page number 0 is out of range")]
        [InlineData("-0", "Range 'to' value out of range in '-0'")]
        [InlineData("0-", "Range 'from' value out of range in '0-'")]
        [InlineData("0-2", "Range 'from' value out of range in '0-2'")]
        [InlineData("2-1", "Range 'to' value cannot be less than range 'from' value in '2-1'")]
        [InlineData("11", "Page number cannot be greater than total number of pages (10) but was 11")]
        [InlineData("-11", "Range 'to' value cannot be greater than total number of pages (10) in '-11'")]
        [InlineData("11-", "Range 'from' value cannot be greater than total number of pages (10) in '11-'")]
        [InlineData("8-11", "Range 'to' value cannot be greater than total number of pages (10) in '8-11'")]
        [InlineData("11-15", "Range 'from' value cannot be greater than total number of pages (10) in '11-15'")]
        public void Create_PageNumberOutOfRange_ShouldThrowPageRangeStringOutOfRangeException(string pageString, string expectedMessage)
        {
            // Act
            Action action = () => PrintStateFactory.Create(null, documentPageCount: 10, pageString);

            // Assert
            action.Should().Throw<PageRangeStringOutOfRangeException>()
                .WithMessage(expectedMessage);
        }
    }
}
