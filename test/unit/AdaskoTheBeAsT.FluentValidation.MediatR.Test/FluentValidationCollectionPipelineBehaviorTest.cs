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

public sealed class FluentValidationCollectionPipelineBehaviorTest
    : IDisposable
{
    private readonly MockRepository _mockRepository;
    private FluentValidationCollectionPipelineBehavior<SampleRequest, SampleResponse>? _sut;
    private IEnumerable<IValidator<SampleRequest>>? _validators;

    public FluentValidationCollectionPipelineBehaviorTest()
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
        _validators = Enumerable.Empty<IValidator<SampleRequest>>();
        _sut = new FluentValidationCollectionPipelineBehavior<SampleRequest, SampleResponse>(_validators);
        var request = new SampleRequest();
        var cancellationToken = CancellationToken.None;
        const RequestHandlerDelegate<SampleResponse>? next = null;

        // Act
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable SA1115 // Parameter should follow comma
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Func<Task> func = async () => await _sut.Handle(
                request,
                next,
                cancellationToken);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore SA1115 // Parameter should follow comma
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        using (new AssertionScope())
        {
            await func.Should().ThrowAsync<ArgumentNullException>();
        }
    }

    [Fact]
    public async Task ShouldProcessCorrectlyWhenNoValidatorUsedAsync()
    {
        // Arrange
        _validators = Enumerable.Empty<IValidator<SampleRequest>>();
        _sut = new FluentValidationCollectionPipelineBehavior<SampleRequest, SampleResponse>(_validators);
        var request = new SampleRequest();
        var cancellationToken = CancellationToken.None;
        static Task<SampleResponse> NextAsync() => Task.FromResult(new SampleResponse { Result = "ok" });

        // Act
        var result = await _sut.Handle(
            request,
            NextAsync,
            cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task ShouldProcessCorrectlyWhenNullValidatorUsedAsync()
    {
        // Arrange
        _validators = new List<IValidator<SampleRequest>> { new NullValidator<SampleRequest>() };
        _sut = new FluentValidationCollectionPipelineBehavior<SampleRequest, SampleResponse>(_validators);
        var request = new SampleRequest();
        var cancellationToken = CancellationToken.None;
        static Task<SampleResponse> NextAsync() => Task.FromResult(new SampleResponse { Result = "ok" });

        // Act
        var result = await _sut.Handle(
            request,
            NextAsync,
            cancellationToken);

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
        _validators = new List<IValidator<SampleRequest>>
        {
            new SampleRequestIdGreaterThanZeroValidator(),
            new SampleRequestNameNonEmptyValidator(),
        };
        _sut = new FluentValidationCollectionPipelineBehavior<SampleRequest, SampleResponse>(_validators);
        var request = new SampleRequest();
        var cancellationToken = CancellationToken.None;
        static Task<SampleResponse> NextAsync() => Task.FromResult(new SampleResponse { Result = "ok" });

        // Act
        Func<Task> func = async () => await _sut.Handle(
            request,
            NextAsync,
            cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            var exception = (await func.Should().ThrowAsync<ValidationException>()).Which;
            exception.Errors.Should().HaveCount(2);
        }
    }
}
