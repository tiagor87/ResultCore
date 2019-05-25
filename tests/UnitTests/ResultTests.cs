using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ResultCore.UnitTests
{
    public class ResultTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void Should_combine_results(int count)
        {
            var results = new List<Result>();
            for (int i = 0; i < count; i++)
            {
                results.Add(Result.Success());
            }

            var result = Result.Combine(results);

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Failure.Should().BeFalse();
            result.Message.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void Should_check_result_and_typed_result_as_IResult()
        {
            var result = Result.Success("Message");
            (result is IResult).Should().BeTrue();

            var typedResult = Result<string>.Success("Message");
            (typedResult is IResult).Should().BeTrue();
        }

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
        public void Should_combine_failure_results_and_typed_results()
        {
            var result = Result.Combine(Result.Fail("Error 1"), Result<long>.Success(1), Result<long>.Fail("Error 2"));

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
        public void Should_combine_successful_results_and_typed_results()
        {
            var result = Result.Combine(Result.Success("Value"), Result<int>.Success(1),
                Result<DateTime>.Success(DateTime.UtcNow));

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
        public void Should_create_failure_result_with_message()
        {
            var result = Result.Fail("Error message");

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Failure.Should().BeTrue();
            result.Message.Should().Be("Error message");
        }

        [Fact]
        public void Should_create_failure_result_with_messages()
        {
            var result = Result.Fail(new string[] {"Error message", "Error message 2"});

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Failure.Should().BeTrue();
            result.Message.Should()
                .Contain("Error message")
                .And
                .Contain("Error message 2");
        }

        [Fact]
        public void Should_create_failure_typed_result_with_message()
        {
            var result = Result<long>.Fail("Error message");

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Failure.Should().BeTrue();
            result.Message.Should().Be("Error message");
        }

        [Fact]
        public void Should_create_failure_typed_result_with_messages()
        {
            var result = Result<long>.Fail(new string[] {"Error message", "Error message 2"});

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Failure.Should().BeTrue();
            result.Message.Should()
                .Contain("Error message")
                .And
                .Contain("Error message 2");
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
        public void Should_get_successful_typed_value()
        {
            const long value = 10;
            var result = Result<long>.Success(value);

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Failure.Should().BeFalse();
            result.Message.Should().BeNullOrWhiteSpace();

            result.Value.Should().Be(value);
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

        [Fact]
        public void Should_get_value_using_as_method_from_typed_result()
        {
            var typedResult = Result<string>.Success("Message");
            typedResult.As<string>().Should().Be("Message");
        }

        [Fact]
        public void Should_implicit_convert_typed_result_to_result()
        {
            const string value = "Value";

            Result result = Result<string>.Success(value);

            result.Successful.Should().BeTrue();
            result.Failure.Should().BeFalse();
            result.Message.Should().BeNullOrWhiteSpace();
            result.Value.Should().BeOfType<string>();
            result.As<string>().Should().Be(value);
        }

        [Fact]
        public void Should_throw_when_combine_more_than_seven_elements()
        {
            Func<Result> action = () => Result.Combine(
                Result.Success(),
                Result.Success(),
                Result.Success(),
                Result.Success(),
                Result.Success(),
                Result.Success(),
                Result.Success(),
                Result.Success());

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void Should_throw_when_get_value_from_failure_in_result()
        {
            var result = Result.Fail("Error message");

            Func<object> action = () => result.Value;

            action.Should().Throw<InvalidOperationException>();
            result.Invoking(x => x.As<object>()).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_throw_when_get_value_from_failure_in_typed_result()
        {
            var result = Result<long>.Fail("Error message");

            Func<long> action = () => result.Value;

            action.Should().Throw<InvalidOperationException>();
        }
    }
}