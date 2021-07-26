using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR.Pipeline;

namespace SampleFMS
{
    public class SampleRequestExceptionHandler//// : IRequestExceptionHandler<SampleRequest, SampleResponse, ValidationException>
    {
#pragma warning disable CC0091 // Use static method
#pragma warning disable CC0057 // Unused parameters
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable VSTHRD200 //Use Async suffix for async methods
        public Task Handle(
            SampleRequest request,
            ValidationException exception,
            RequestExceptionHandlerState<SampleResponse> state,
            CancellationToken cancellationToken)
        {
            if (exception is null)
            {
                return Task.CompletedTask;
            }

            foreach (var error in exception.Errors)
            {
                Console.WriteLine(error);
            }

            return Task.CompletedTask;
        }
    }
#pragma warning restore VSTHRD200
#pragma warning restore CA1801 // Review unused parameters
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CC0057 // Unused parameters
#pragma warning restore CC0091 // Use static method
}
