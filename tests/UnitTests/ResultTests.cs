using System;
using FluentAssertions;
using ResultCore;
using Xunit;

namespace UnitTests
{
    public class ResultTests
    {
        [Fact]
        public void Should_combine_failure_results()
        {
            var result = Result.Combine(Result.Fail("Error 1"), Result.Success(1), Result.Fail("Error 2"));

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Failure.Should().BeTrue();
            result.Message.Should().NotBeNullOrWhiteSpace();
            result.Message.Should()
                .Contain("Error 1")
                .And
                .Contain("Error 2");
        }

        [Fact]
        public void Should_combine_successful_results()
        {
            var result = Result.Combine(Result.Success("Value"), Result.Success(1), Result.Success(DateTime.UtcNow));

            var (text, number, date) = result.As<(string, int, DateTime)>();
            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Failure.Should().BeFalse();
            result.Message.Should().BeNullOrWhiteSpace();
            text.Should().Be("Value");
            number.Should().Be(1);
            date.Should().BeCloseTo(date, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Should_create_failure_result()
        {
            var result = Result.Fail("Error message");

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Failure.Should().BeTrue();
            result.Message.Should().Be("Error message");
        }

        [Fact]
        public void Should_create_successful_result()
        {
            var result = Result.Success();

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Failure.Should().BeFalse();
            result.Message.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void Should_get_successful_value()
        {
            var result = Result.Success("Value");

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Failure.Should().BeFalse();
            result.Message.Should().BeNullOrWhiteSpace();
            result.As<string>().Should().Be("Value");
        }
    }
}