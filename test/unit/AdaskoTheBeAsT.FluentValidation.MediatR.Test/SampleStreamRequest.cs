using MediatR;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

public class SampleStreamRequest
    : IStreamRequest<SampleStreamResponse>
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Id { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Name { get; set; }
}
