using MediatR;

namespace SampleFMS
{
    public class SampleRequest : IRequest<SampleResponse>
    {
        public int Id { get; set; }
    }
}
