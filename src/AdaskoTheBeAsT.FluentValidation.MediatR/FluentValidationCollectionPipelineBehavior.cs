using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace AdaskoTheBeAsT.FluentValidation.MediatR
{
    public class FluentValidationCollectionPipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public FluentValidationCollectionPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
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
            var context = new ValidationContext<TRequest>(request);
            var validationResultTasks = _validators
                .Select(async v => await v.ValidateAsync(context, cancellationToken).ConfigureAwait(false));

            var validationResults = await Task.WhenAll(validationResultTasks).ConfigureAwait(false);

            var failures = validationResults.FirstOrDefault()?
                .Errors
                .Where(f => f != null)
                .ToList();

            if (failures?.Count > 0)
            {
                throw new ValidationException(failures);
            }

            return await next!().ConfigureAwait(false);
        }
    }
}
