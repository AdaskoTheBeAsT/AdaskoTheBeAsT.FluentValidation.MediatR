using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SampleFMS
{
    public class SampleRequestHandler : IRequestHandler<SampleRequest, SampleResponse>
    {
        public Task<SampleResponse> Handle(SampleRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return Task.FromResult(new SampleResponse { Id = request.Id });
        }
    }
}
