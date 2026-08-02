using System;
using AwesomeAssertions;
using Xunit;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

public class TestEnvironmentTests
{
    [Fact]
    public void ShouldProvideMediatRLicenseKeyToTestProcess()
    {
        // The module initializer of TestEnvironment runs before any test, so the key comes
        // either from the local .env file or from the environment injected by CI.
        Environment.GetEnvironmentVariable("MEDIATR_LICENSE_KEY").Should().NotBeNullOrEmpty();
    }
}
