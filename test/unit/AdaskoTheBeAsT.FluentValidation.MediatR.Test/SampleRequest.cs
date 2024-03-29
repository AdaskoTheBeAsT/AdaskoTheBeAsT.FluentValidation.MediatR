using MediatR;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

public class SampleRequest
    : IRequest<SampleResponse>
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Id { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Name { get; set; }
}
