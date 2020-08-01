# AdaskoTheBeAsT.FluentValidation.MediatR

FluentValidation behavior for MediatR

## Badges

[![Build Status](https://adaskothebeast.visualstudio.com/AdaskoTheBeAsT.FluentValidation.MediatR/_apis/build/status/AdaskoTheBeAsT.AdaskoTheBeAsT.FluentValidation.MediatR?branchName=master)](https://adaskothebeast.visualstudio.com/AdaskoTheBeAsT.FluentValidation.MediatR/_build/latest?definitionId=9&branchName=master)
![Azure DevOps tests](https://img.shields.io/azure-devops/tests/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR/10)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR/10?style=plastic)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR&metric=alert_status)](https://sonarcloud.io/dashboard?id=AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR)
![Sonar Tests](https://img.shields.io/sonar/tests/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Test Count](https://img.shields.io/sonar/total_tests/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Test Execution Time](https://img.shields.io/sonar/test_execution_time/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Coverage](https://img.shields.io/sonar/coverage/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io&style=plastic)
![Nuget](https://img.shields.io/nuget/dt/AdaskoTheBeAsT.FluentValidation.MediatR)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FAdaskoTheBeAsT%2FAdaskoTheBeAsT.FluentValidation.MediatR.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FAdaskoTheBeAsT%2FAdaskoTheBeAsT.FluentValidation.MediatR?ref=badge_shield)

## Usage

It can be used in combination with [AdaskoTheBeAsT.FluentValidation.SimpleInjector](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.SimpleInjector) [AdaskoTheBeAsT.MediatR.SimpleInjector](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.MediatR.SimpleInjector)

```cs
    container.AddFluentValidation(
        cfg =>
        {
            cfg.WithAssembliesToScan(assemblies);
            cfg.AsScoped();
        });

    container.AddMediatR(
        cfg =>
        {
            cfg.WithAssembliesToScan(assemblies);
            cfg.UsingBuiltinPipelineProcessorBehaviors(true);
            cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationPipelineBehavior<,>));
        });
```

## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FAdaskoTheBeAsT%2FAdaskoTheBeAsT.FluentValidation.MediatR.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FAdaskoTheBeAsT%2FAdaskoTheBeAsT.FluentValidation.MediatR?ref=badge_large)