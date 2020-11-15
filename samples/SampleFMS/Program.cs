using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AdaskoTheBeAsT.FluentValidation.MediatR;
using AdaskoTheBeAsT.FluentValidation.SimpleInjector;
using AdaskoTheBeAsT.MediatR.SimpleInjector;
using MediatR;
using SimpleInjector;

namespace SampleFMS
{
    internal static class Program
    {
        private static async Task Main()
        {
#pragma warning disable CC0022 // Should dispose object
#pragma warning disable MA0004 // Use .ConfigureAwait(false)
            await using var container = new Container();
#pragma warning restore MA0004 // Use .ConfigureAwait(false)
#pragma warning restore CC0022 // Should dispose object

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var mainAssembly = typeof(Program).Assembly;
            var assemblies = new List<Assembly> { mainAssembly };

            var refAssemblies = mainAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in refAssemblies
                .Where(a => a.FullName.StartsWith(nameof(SampleFMS), StringComparison.OrdinalIgnoreCase)))
            {
                var assembly = loadedAssemblies.Find(l => l.FullName!.Equals(assemblyName.FullName, StringComparison.OrdinalIgnoreCase))
                               ?? AppDomain.CurrentDomain.Load(assemblyName);
                assemblies.Add(assembly);
            }

            container.AddFluentValidation(
                cfg =>
                {
                    cfg.WithAssembliesToScan(assemblies);
                    cfg.RegisterAsValidatorCollection();
                });

            container.AddMediatR(
                cfg =>
                {
                    cfg.WithAssembliesToScan(assemblies);
                    cfg.UsingBuiltinPipelineProcessorBehaviors(true);
                    cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationCollectionPipelineBehavior<,>));
                });

            var mediator = container.GetInstance<IMediator>();
            var request = new SampleRequest { Id = 9 };
            var response = await mediator.Send(request).ConfigureAwait(false);
            var request2 = new SampleRequest2 { Id = 9 };
            var response2 = await mediator.Send(request2).ConfigureAwait(false);

            Console.WriteLine(response.Id);
            Console.WriteLine(response2.Id);
        }
    }
}
