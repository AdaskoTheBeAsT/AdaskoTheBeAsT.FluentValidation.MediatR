using MediatR;

namespace SampleFMS
{
    public class SampleRequest2 : IRequest<SampleResponse2>
    {
        public int Id { get; set; }
    }
}
