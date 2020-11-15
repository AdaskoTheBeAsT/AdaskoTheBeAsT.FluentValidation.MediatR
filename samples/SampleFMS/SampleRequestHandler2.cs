using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SampleFMS
{
    public class SampleRequestHandler2 : IRequestHandler<SampleRequest2, SampleResponse2>
    {
        public Task<SampleResponse2> Handle(SampleRequest2 request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return Task.FromResult(new SampleResponse2 { Id = request.Id });
        }
    }
}
