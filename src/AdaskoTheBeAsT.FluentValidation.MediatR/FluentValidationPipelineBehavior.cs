using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace AdaskoTheBeAsT.FluentValidation.MediatR;

public class FluentValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IValidator<TRequest>? _validator;

    public FluentValidationPipelineBehavior(IValidator<TRequest>? validator)
    {
        _validator = validator;
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (next is null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        return HandleInternalAsync(request, next, cancellationToken);
    }

    internal async Task<TResponse> HandleInternalAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validator != null)
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResult = await _validator.ValidateAsync(context, cancellationToken).ConfigureAwait(false);

            var failures = validationResult.Errors.ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

#pragma warning disable CC0031 // Check for null before calling a delegate
        return await next().ConfigureAwait(false);
#pragma warning restore CC0031 // Check for null before calling a delegate
    }
}
