using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace AdaskoTheBeAsT.FluentValidation.MediatR;

public class FluentValidationStreamPipelineBehavior<TRequest, TResponse>
    : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public FluentValidationStreamPipelineBehavior(IValidator<TRequest>? validator)
    {
        _validator = validator;
    }

    public IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        return HandleInternalAsync(request, next, cancellationToken);
    }

    internal async IAsyncEnumerable<TResponse> HandleInternalAsync(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        [EnumeratorCancellation] CancellationToken cancellationToken)
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
        await foreach (var result in next().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return result;
        }
#pragma warning restore CC0031 // Check for null before calling a delegate
    }
}
