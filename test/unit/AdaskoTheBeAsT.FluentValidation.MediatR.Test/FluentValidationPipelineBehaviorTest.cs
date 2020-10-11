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

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test
{
    public sealed class FluentValidationPipelineBehaviorTest
        : IDisposable
    {
        private readonly MockRepository _mockRepository;
        private FluentValidationPipelineBehavior<SampleRequest, SampleResponse>? _sut;
        private IEnumerable<IValidator<SampleRequest>>? _validators;

        public FluentValidationPipelineBehaviorTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
        }

        public void Dispose()
        {
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void ShouldThrowExceptionWhenNullAsNextWasPassed()
        {
            // Arrange
            _validators = Enumerable.Empty<IValidator<SampleRequest>>();
            _sut = new FluentValidationPipelineBehavior<SampleRequest, SampleResponse>(_validators);
            var request = new SampleRequest();
            var cancellationToken = CancellationToken.None;
            const RequestHandlerDelegate<SampleResponse>? next = null;

            // Act
#pragma warning disable SA1115 // Parameter should follow comma
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Func<Task> func = async () => await _sut.Handle(
                request,
                cancellationToken,

                // ReSharper disable once ExpressionIsAlwaysNull
                next).ConfigureAwait(false);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore SA1115 // Parameter should follow comma

            // Assert
            using (new AssertionScope())
            {
                func.Should().ThrowAsync<ArgumentNullException>();
            }
        }

        [Fact]
        public async Task ShouldProcessCorrectlyWhenNoValidatorUsedAsync()
        {
            // Arrange
            _validators = Enumerable.Empty<IValidator<SampleRequest>>();
            _sut = new FluentValidationPipelineBehavior<SampleRequest, SampleResponse>(_validators);
            var request = new SampleRequest();
            var cancellationToken = CancellationToken.None;
            static Task<SampleResponse> Next() => Task.FromResult(new SampleResponse { Result = "ok" });

            // Act
            var result = await _sut.Handle(
                request,
                cancellationToken,
                Next);

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
            _sut = new FluentValidationPipelineBehavior<SampleRequest, SampleResponse>(_validators);
            var request = new SampleRequest();
            var cancellationToken = CancellationToken.None;
            static Task<SampleResponse> Next() => Task.FromResult(new SampleResponse { Result = "ok" });

            // Act
            var result = await _sut.Handle(
                request,
                cancellationToken,
                Next);

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
            _sut = new FluentValidationPipelineBehavior<SampleRequest, SampleResponse>(_validators);
            var request = new SampleRequest();
            var cancellationToken = CancellationToken.None;
            static Task<SampleResponse> Next() => Task.FromResult(new SampleResponse { Result = "ok" });

            // Act
            Func<Task> func = async () => await _sut.Handle(
                request,
                cancellationToken,
                Next).ConfigureAwait(false);

            // Assert
            using (new AssertionScope())
            {
                var exception = await func.Should().ThrowAsync<ValidationException>().ConfigureAwait(false);
                exception.Which.Errors.Should().HaveCount(2);
            }
        }
    }
}
