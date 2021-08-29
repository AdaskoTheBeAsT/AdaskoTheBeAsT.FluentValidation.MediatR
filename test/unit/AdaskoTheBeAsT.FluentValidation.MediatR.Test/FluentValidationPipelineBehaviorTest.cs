using System;
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
        private IValidator<SampleRequest>? _validator;

        public FluentValidationPipelineBehaviorTest()
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
            _sut = new FluentValidationPipelineBehavior<SampleRequest, SampleResponse>(_validator);
            var request = new SampleRequest();
            var cancellationToken = CancellationToken.None;
            const RequestHandlerDelegate<SampleResponse>? next = null;

            // Act
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable SA1115 // Parameter should follow comma
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Func<Task> func = async () => await _sut.Handle(
                request,
                cancellationToken,
                next)
                .ConfigureAwait(false);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore SA1115 // Parameter should follow comma
#pragma warning restore CS8604 // Possible null reference argument.

            // Assert
            using (new AssertionScope())
            {
                await func.Should().ThrowAsync<ArgumentNullException>().ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task ShouldProcessCorrectlyWhenNoValidatorUsedAsync()
        {
            // Arrange
            _sut = new FluentValidationPipelineBehavior<SampleRequest, SampleResponse>(null);
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
            _validator = new NullValidator<SampleRequest>();
            _sut = new FluentValidationPipelineBehavior<SampleRequest, SampleResponse>(_validator);
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
            _validator = new SampleRequestIdGreaterThanZeroValidator();
            _sut = new FluentValidationPipelineBehavior<SampleRequest, SampleResponse>(_validator);
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
                var exception = (await func.Should().ThrowAsync<ValidationException>().ConfigureAwait(false)).Which;
                exception.Errors.Should().HaveCount(1);
            }
        }
    }
}
