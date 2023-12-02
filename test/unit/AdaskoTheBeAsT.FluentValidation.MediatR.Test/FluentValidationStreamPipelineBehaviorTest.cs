using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using MediatR;
using Moq;
using Xunit;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

public sealed class FluentValidationStreamPipelineBehaviorTest
    : IDisposable
{
    private readonly MockRepository _mockRepository;
    private FluentValidationStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>? _sut;
    private IValidator<SampleStreamRequest>? _validator;

    public FluentValidationStreamPipelineBehaviorTest()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);
    }

    public void Dispose()
    {
        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenNullAsNextWasPassedAsync()
    {
        // Arrange
        _sut = new FluentValidationStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>(_validator);
        var request = new SampleStreamRequest();
        var cancellationToken = CancellationToken.None;
        const StreamHandlerDelegate<SampleStreamResponse>? next = null;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable SA1115 // Parameter should follow comma
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Func<Task<SampleStreamResponse?>> func = async () =>
        {
            SampleStreamResponse? response = null;
            await foreach (var item in _sut.Handle(
                    request,
                    next,
                    cancellationToken)
                .ConfigureAwait(false))
            {
                response = item;
            }

            return response;
        };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore SA1115 // Parameter should follow comma
#pragma warning restore CS8604 // Possible null reference argument.

        // Act and Assert
        using (new AssertionScope())
        {
            await func.Should().ThrowAsync<ArgumentNullException>();
        }
    }

    [Fact]
    public async Task ShouldProcessCorrectlyWhenNoValidatorUsedAsync()
    {
        // Arrange
        _sut = new FluentValidationStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>(validator: null);
        var request = new SampleStreamRequest();
        var cancellationToken = CancellationToken.None;
        var response = new SampleStreamResponse { Result = "ok" };
        IAsyncEnumerable<SampleStreamResponse> Next() => new[] { response }.ToAsyncEnumerable();
        async Task<SampleStreamResponse?> Func()
        {
            SampleStreamResponse? resp = null;
            await foreach (var item in _sut.Handle(
                                   request,
                                   Next,
                                   cancellationToken)
                               .ConfigureAwait(false))
            {
                resp = item;
            }

            return resp;
        }

        // Act
        var result = await Func();

        // Assert
        using (new AssertionScope())
        {
            result.Should().Be(response);
        }
    }

    [Fact]
    public async Task ShouldProcessCorrectlyWhenNullValidatorUsedAsync()
    {
        // Arrange
        _validator = new NullValidator<SampleStreamRequest>();
        _sut = new FluentValidationStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>(_validator);
        var request = new SampleStreamRequest();
        var cancellationToken = CancellationToken.None;
        var response = new SampleStreamResponse { Result = "ok" };
        IAsyncEnumerable<SampleStreamResponse> Next() => new[] { response }.ToAsyncEnumerable();
        async Task<SampleStreamResponse?> Func()
        {
            SampleStreamResponse? resp = null;
            await foreach (var item in _sut.Handle(
                                   request,
                                   Next,
                                   cancellationToken)
                               .ConfigureAwait(false))
            {
                resp = item;
            }

            return resp;
        }

        // Act
        var result = await Func();

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task ShouldReturnErrorsWhenInvalidRequestPassedAsync()
    {
        // Arrange
        _validator = new SampleStreamRequestIdGreaterThanZeroValidator();
        _sut = new FluentValidationStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>(_validator);
        var request = new SampleStreamRequest();
        var cancellationToken = CancellationToken.None;
        var response = new SampleStreamResponse { Result = "ok" };
        IAsyncEnumerable<SampleStreamResponse> Next() => new[] { response }.ToAsyncEnumerable();
        Func<Task<SampleStreamResponse?>> func = async () =>
        {
            SampleStreamResponse? resp = null;
            await foreach (var item in _sut.Handle(
                                   request,
                                   Next,
                                   cancellationToken)
                               .ConfigureAwait(false))
            {
                resp = item;
            }

            return resp;
        };

        // Assert
        using (new AssertionScope())
        {
            var exception = (await func.Should().ThrowAsync<ValidationException>()).Which;
            exception.Errors.Should().ContainSingle();
        }
    }
}
