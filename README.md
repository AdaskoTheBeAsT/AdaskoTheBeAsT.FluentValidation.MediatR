# AdaskoTheBeAsT.FluentValidation.MediatR

FluentValidation behavior for MediatR

## Badges

[![CodeFactor](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.fluentvalidation.mediatr/badge)](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.fluentvalidation.mediatr)
[![Total alerts](https://img.shields.io/lgtm/alerts/g/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR/alerts/)
[![Build Status](https://adaskothebeast.visualstudio.com/AdaskoTheBeAsT.FluentValidation.MediatR/_apis/build/status/AdaskoTheBeAsT.AdaskoTheBeAsT.FluentValidation.MediatR?branchName=master)](https://adaskothebeast.visualstudio.com/AdaskoTheBeAsT.FluentValidation.MediatR/_build/latest?definitionId=9&branchName=master)
![Azure DevOps tests](https://img.shields.io/azure-devops/tests/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR/15)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR/15?style=plastic)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR&metric=alert_status)](https://sonarcloud.io/dashboard?id=AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR)
![Sonar Tests](https://img.shields.io/sonar/tests/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Test Count](https://img.shields.io/sonar/total_tests/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Test Execution Time](https://img.shields.io/sonar/test_execution_time/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Coverage](https://img.shields.io/sonar/coverage/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io&style=plastic)
![Nuget](https://img.shields.io/nuget/dt/AdaskoTheBeAsT.FluentValidation.MediatR)

## Usage

It can be used in combination with [AdaskoTheBeAsT.FluentValidation.SimpleInjector](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.SimpleInjector) [AdaskoTheBeAsT.MediatR.SimpleInjector](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.MediatR.SimpleInjector)

### Validators registered as single
There should be only one validator per target
If there is multiple combined validators needed then prepare one which will gather all rules from other
based  on [Fluent Validation Including Rules](https://docs.fluentvalidation.net/en/latest/including-rules.html)
and mark all sub validators with attribute SkipValidatorRegistrationAttribute.

```cs
    container.AddFluentValidation(
        cfg =>
        {
            cfg.WithAssembliesToScan(assemblies);
            cfg.AsScoped();
            cfg.RegisterAsSingleValidator(); // can be skipped as it is default
        });

    container.AddMediatR(
        cfg =>
        {
            cfg.WithAssembliesToScan(assemblies);
            cfg.UsingBuiltinPipelineProcessorBehaviors(true);
            cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationPipelineBehavior<,>));
        });
```

### Validators registered as collection

```cs
    container.AddFluentValidation(
        cfg =>
        {
            cfg.WithAssembliesToScan(assemblies);
            cfg.AsScoped();
            cfg.RegisterAsValidatorCollection();
        });

    container.AddMediatR(
        cfg =>
        {
            cfg.WithAssembliesToScan(assemblies);
            cfg.UsingBuiltinPipelineProcessorBehaviors(true);
            cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationCollectionPipelineBehavior<,>));
        });
```