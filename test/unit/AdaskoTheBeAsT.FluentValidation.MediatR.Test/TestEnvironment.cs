using System.Runtime.CompilerServices;
using dotenv.net;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

/// <summary>
/// Loads the git ignored <c>.env</c> file from the repository root into the test process,
/// so that locally executed tests see the same environment variables that CI injects from
/// GitHub secrets. MediatR reads <c>MEDIATR_LICENSE_KEY</c> from the environment,
/// see <c>MediatR.Licensing.LicenseAccessor.ResolveLicenseKey</c>.
/// </summary>
internal static class TestEnvironment
{
    // Runs before any test resolves IMediator, which is when the license key is read.
    [ModuleInitializer]
    internal static void Load() =>
        DotEnv.Fluent()

            // Probing starts at AppContext.BaseDirectory, which is
            // test/unit/<project>/bin/<configuration>/<tfm>, so the repository root holding
            // .env is six levels up. The library default of four levels is not enough.
            .WithProbeForEnv(probeLevelsToSearch: 8)

            // Values injected by CI must win over a developer's local file.
            .WithoutOverwriteExistingVars()
            .WithTrimValues()
            .Load();
}
