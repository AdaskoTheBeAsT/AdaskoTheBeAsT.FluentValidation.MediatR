using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace AdaskoTheBeAsT.FluentValidation.MediatR;

public class FluentValidationCollectionStreamPipelineBehavior<TRequest, TResponse>
    : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public FluentValidationCollectionStreamPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        StreamHandlerDelegate<TResponse> next)
    {
        if (next is null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        return HandleInternalAsync(request, next, cancellationToken);
    }

    internal async IAsyncEnumerable<TResponse> HandleInternalAsync(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationResultTasks = _validators
            .Select(async v => await v.ValidateAsync(context, cancellationToken).ConfigureAwait(false));

        var validationResults = await Task.WhenAll(validationResultTasks).ConfigureAwait(false);

        var failures = validationResults.FirstOrDefault()
            ?.Errors
            .Where(f => f != null)
            .ToList();

        if (failures?.Count > 0)
        {
            throw new ValidationException(failures);
        }

#pragma warning disable CC0031 // Check for null before calling a delegate
        await foreach (var result in next().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return result;
        }
#pragma warning restore CC0031 // Check for null before calling a delegate
    }
}
