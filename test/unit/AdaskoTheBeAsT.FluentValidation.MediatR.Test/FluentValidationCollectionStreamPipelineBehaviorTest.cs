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

public sealed class FluentValidationCollectionStreamPipelineBehaviorTest
    : IDisposable
{
    private readonly MockRepository _mockRepository;
    private FluentValidationCollectionStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>? _sut;
    private IEnumerable<IValidator<SampleStreamRequest>>? _validators;

    public FluentValidationCollectionStreamPipelineBehaviorTest()
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
        _validators = Enumerable.Empty<IValidator<SampleStreamRequest>>();
        _sut = new FluentValidationCollectionStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>(_validators);
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
                                   cancellationToken,
                                   next)
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
            await func.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
        }
    }

    [Fact]
    public async Task ShouldProcessCorrectlyWhenNoValidatorUsedAsync()
    {
        // Arrange
        _validators = Enumerable.Empty<IValidator<SampleStreamRequest>>();
        _sut = new FluentValidationCollectionStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>(_validators);
        var request = new SampleStreamRequest();
        var cancellationToken = CancellationToken.None;
        var response = new SampleStreamResponse { Result = "ok" };
        IAsyncEnumerable<SampleStreamResponse> Next() => new[] { response }.ToAsyncEnumerable();
        async Task<SampleStreamResponse?> Func()
        {
            SampleStreamResponse? resp = null;
            await foreach (var item in _sut.Handle(
                                   request,
                                   cancellationToken,
                                   Next)
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
        _validators = new List<IValidator<SampleStreamRequest>> { new NullValidator<SampleStreamRequest>() };
        _sut = new FluentValidationCollectionStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>(_validators);
        var request = new SampleStreamRequest();
        var cancellationToken = CancellationToken.None;
        var response = new SampleStreamResponse { Result = "ok" };
        IAsyncEnumerable<SampleStreamResponse> Next() => new[] { response }.ToAsyncEnumerable();
        async Task<SampleStreamResponse?> Func()
        {
            SampleStreamResponse? resp = null;
            await foreach (var item in _sut.Handle(
                                   request,
                                   cancellationToken,
                                   Next)
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
        _validators = new List<IValidator<SampleStreamRequest>>
        {
            new SampleStreamRequestIdGreaterThanZeroValidator(),
            new SampleStreamRequestNameNonEmptyValidator(),
        };
        _sut = new FluentValidationCollectionStreamPipelineBehavior<SampleStreamRequest, SampleStreamResponse>(_validators);
        var request = new SampleStreamRequest();
        var cancellationToken = CancellationToken.None;
        var response = new SampleStreamResponse { Result = "ok" };
        IAsyncEnumerable<SampleStreamResponse> Next() => new[] { response }.ToAsyncEnumerable();
        Func<Task<SampleStreamResponse?>> func = async () =>
        {
            SampleStreamResponse? resp = null;
            await foreach (var item in _sut.Handle(
                                   request,
                                   cancellationToken,
                                   Next)
                               .ConfigureAwait(false))
            {
                resp = item;
            }

            return resp;
        };

        // Assert
        using (new AssertionScope())
        {
            var exception = (await func.Should().ThrowAsync<ValidationException>().ConfigureAwait(false)).Which;
            exception.Errors.Should().HaveCount(2);
        }
    }
}
