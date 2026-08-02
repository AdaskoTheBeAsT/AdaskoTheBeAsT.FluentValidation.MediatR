# AdaskoTheBeAsT.FluentValidation.MediatR

**Seamless FluentValidation integration for MediatR pipeline - automatic request validation before your handlers execute.**

[![CodeFactor](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.fluentvalidation.mediatr/badge)](https://www.codefactor.io/repository/github/adaskothebeast/adaskothebeast.fluentvalidation.mediatr)
[![Build Status](https://img.shields.io/azure-devops/build/adaskothebeast/AdaskoTheBeAsT.FluentValidation.MediatR/15)](https://img.shields.io/azure-devops/build/adaskothebeast/AdaskoTheBeAsT.FluentValidation.MediatR/15)
![Azure DevOps tests](https://img.shields.io/azure-devops/tests/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR/15)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR/15?style=plastic)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR&metric=alert_status)](https://sonarcloud.io/dashboard?id=AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR)
![Sonar Coverage](https://img.shields.io/sonar/coverage/AdaskoTheBeAsT_AdaskoTheBeAsT.FluentValidation.MediatR?server=https%3A%2F%2Fsonarcloud.io&style=plastic)
![Nuget](https://img.shields.io/nuget/dt/AdaskoTheBeAsT.FluentValidation.MediatR)
![Nuget](https://img.shields.io/nuget/v/AdaskoTheBeAsT.FluentValidation.MediatR)

---

## 🚀 Why This Library?

Stop writing validation code in every MediatR handler! This library automatically validates all your MediatR requests using FluentValidation **before** they reach your handlers. 

**Benefits:**
- ✅ **Automatic validation** - Set it up once, validate everywhere
- ✅ **Clean handlers** - Keep business logic separate from validation
- ✅ **Fail fast** - Invalid requests never reach your handlers
- ✅ **DRY principle** - No boilerplate validation code in handlers
- ✅ **Streaming support** - Works with both regular and streaming MediatR requests
- ✅ **Flexible** - Single validator or multiple validators per request

---

## 📦 Installation

```bash
dotnet add package AdaskoTheBeAsT.FluentValidation.MediatR
```

**Supported frameworks:**
- .NET 10.0
- .NET 9.0
- .NET 8.0

**Dependencies:**
- MediatR 14.2.0+
- FluentValidation 12.1.0+

---

## ⚡ Quick Start

### Step 1: Create a Request and Validator

```csharp
using FluentValidation;
using MediatR;

// Your MediatR request
public class CreateUserCommand : IRequest<UserResponse>
{
    public string Username { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// Your FluentValidation validator
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18)
            .WithMessage("User must be at least 18 years old");
    }
}
```

### Step 2: Register with SimpleInjector

```csharp
using AdaskoTheBeAsT.FluentValidation.MediatR;
using AdaskoTheBeAsT.FluentValidation.SimpleInjector;
using AdaskoTheBeAsT.MediatR.SimpleInjector;
using SimpleInjector;

var container = new Container();
var assemblies = new[] { typeof(Program).Assembly };

// Register FluentValidation validators
container.AddFluentValidation(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
    cfg.AsScoped();
    cfg.RegisterAsSingleValidator(); // Default: one validator per request type
});

// Register MediatR with validation pipeline
container.AddMediatR(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
    cfg.UsingBuiltinPipelineProcessorBehaviors(true);
    cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationPipelineBehavior<,>));
    cfg.UsingStreamPipelineBehaviors(typeof(FluentValidationStreamPipelineBehavior<,>));
});
```

### Step 3: Use It!

```csharp
var mediator = container.GetInstance<IMediator>();

var command = new CreateUserCommand
{
    Username = "john_doe",
    Email = "john@example.com",
    Age = 25
};

try
{
    // Validation happens automatically before handler execution
    var response = await mediator.Send(command);
    Console.WriteLine($"User created: {response.Id}");
}
catch (ValidationException ex)
{
    // Handle validation failures
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}
```

---

## 📖 Detailed Usage

### Approach 1: Single Validator (Recommended)

**Use when:** You have one validator per request type, or you want to combine multiple validators into one.

```csharp
container.AddFluentValidation(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
    cfg.AsScoped();
    cfg.RegisterAsSingleValidator(); // Default - can be omitted
});

container.AddMediatR(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
    cfg.UsingBuiltinPipelineProcessorBehaviors(true);
    cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationPipelineBehavior<,>));
    cfg.UsingStreamPipelineBehaviors(typeof(FluentValidationStreamPipelineBehavior<,>));
});
```

**Combining multiple validators:**

If you need multiple validation rule sets, create a composite validator:

```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        // Include rules from other validators
        Include(new UserNameValidationRules());
        Include(new EmailValidationRules());
        Include(new AgeValidationRules());
    }
}

// Mark sub-validators to prevent duplicate registration
[SkipValidatorRegistration]
public class UserNameValidationRules : AbstractValidator<CreateUserCommand>
{
    public UserNameValidationRules()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
    }
}

[SkipValidatorRegistration]
public class EmailValidationRules : AbstractValidator<CreateUserCommand>
{
    public EmailValidationRules()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
```

📚 [Learn more about including rules](https://docs.fluentvalidation.net/en/latest/including-rules.html)

---

### Approach 2: Multiple Validators (Collection)

**Use when:** You want to maintain separate, independent validators for the same request type.

```csharp
container.AddFluentValidation(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
    cfg.AsScoped();
    cfg.RegisterAsValidatorCollection(); // Register multiple validators
});

container.AddMediatR(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
    cfg.UsingBuiltinPipelineProcessorBehaviors(true);
    cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationCollectionPipelineBehavior<,>));
    cfg.UsingStreamPipelineBehaviors(typeof(FluentValidationCollectionStreamPipelineBehavior<,>));
});
```

**Example with multiple independent validators:**

```csharp
// Validator 1: Business rules
public class CreateUserBusinessRulesValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserBusinessRulesValidator()
    {
        RuleFor(x => x.Age).GreaterThanOrEqualTo(18);
    }
}

// Validator 2: Data format rules
public class CreateUserDataValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserDataValidator()
    {
        RuleFor(x => x.Username).NotEmpty().Matches("^[a-zA-Z0-9_]+$");
        RuleFor(x => x.Email).EmailAddress();
    }
}

// Both validators run, all errors from both are collected
```

**⚠️ Note:** All validators run in parallel, and **all validation errors** from **all validators** are collected and thrown together.

---

### Streaming Requests

Works seamlessly with MediatR streaming requests:

```csharp
public class StreamDataQuery : IStreamRequest<DataChunk>
{
    public string Filter { get; set; }
    public int PageSize { get; set; }
}

public class StreamDataQueryValidator : AbstractValidator<StreamDataQuery>
{
    public StreamDataQueryValidator()
    {
        RuleFor(x => x.Filter).NotEmpty();
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

// Validator runs BEFORE streaming starts
await foreach (var chunk in mediator.CreateStream(new StreamDataQuery { Filter = "test", PageSize = 50 }))
{
    Console.WriteLine(chunk);
}
```

---

## 🔧 Configuration Options

### Validator Lifetime

```csharp
container.AddFluentValidation(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
    cfg.AsScoped();        // Scoped (default, recommended)
    // cfg.AsSingleton();  // Singleton (if validators are stateless)
    // cfg.AsTransient();  // Transient (new instance each time)
});
```

### Assembly Scanning

```csharp
// Scan multiple assemblies
var assemblies = new[]
{
    typeof(Program).Assembly,
    typeof(CreateUserCommand).Assembly,
    typeof(OrderModule).Assembly
};

container.AddFluentValidation(cfg =>
{
    cfg.WithAssembliesToScan(assemblies);
});
```

---

## 🎯 When Validation Occurs

```
┌─────────────────────────────────────────────────────┐
│  1. Client sends request                            │
│     mediator.Send(command)                          │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│  2. MediatR Pipeline: Validation Behavior           │
│     ✓ FluentValidationPipelineBehavior runs         │
│     ✓ All validators execute                        │
│     ✓ Errors collected                              │
└──────────────────┬──────────────────────────────────┘
                   │
         ┌─────────┴─────────┐
         │                   │
    Invalid             Valid
         │                   │
         ▼                   ▼
┌────────────────┐  ┌────────────────────────────────┐
│ ValidationEx-  │  │ 3. Your Handler Executes       │
│ ception thrown │  │    Handler<TRequest, TResponse>│
│                │  │    ✓ Request is guaranteed     │
│ Handler never  │  │      to be valid               │
│ executes       │  │                                │
└────────────────┘  └────────────────────────────────┘
```

---

## 🐛 Troubleshooting

### Validation Not Running

**Problem:** Validators are not executing.

**Solutions:**
1. ✅ Ensure validators are in scanned assemblies
2. ✅ Verify pipeline behavior is registered: `UsingPipelineProcessorBehaviors(typeof(FluentValidationPipelineBehavior<,>))`
3. ✅ Check validator implements `AbstractValidator<TRequest>` or `IValidator<TRequest>`
4. ✅ Ensure validator is not decorated with `[SkipValidatorRegistration]`

### No Validators Found

```csharp
// ❌ Wrong - assembly not included
cfg.WithAssembliesToScan(Array.Empty<Assembly>());

// ✅ Correct - include assemblies with validators
cfg.WithAssembliesToScan(new[] { typeof(CreateUserCommandValidator).Assembly });
```

### ValidationException Not Caught

```csharp
using FluentValidation; // ✅ Make sure to use this namespace

try
{
    await mediator.Send(command);
}
catch (ValidationException ex) // FluentValidation.ValidationException
{
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}
```

### Multiple Validators Only Showing Some Errors

**If using collection approach**, ensure you're using the **Collection** pipeline behaviors:

```csharp
// ❌ Wrong - only uses first validator
cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationPipelineBehavior<,>));

// ✅ Correct - uses all validators
cfg.UsingPipelineProcessorBehaviors(typeof(FluentValidationCollectionPipelineBehavior<,>));
```

---

## 🏗️ Architecture

```
MediatR Request → Validation Pipeline Behavior → Handler
                         ↓
                  IValidator<TRequest>
                         ↓
                  Validation passes? 
                    ↙         ↘
                  Yes          No
                   ↓            ↓
            Call next()    Throw ValidationException
                   ↓
            Handler executes
```

---

## 🤝 Related Packages

This library works great with:
- **[AdaskoTheBeAsT.FluentValidation.SimpleInjector](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.SimpleInjector)** - FluentValidation registration for SimpleInjector
- **[AdaskoTheBeAsT.MediatR.SimpleInjector](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.MediatR.SimpleInjector)** - MediatR registration for SimpleInjector

---

## 📝 Best Practices

1. **Use Single Validator approach** when possible - simpler and more maintainable
2. **Fail fast** - Put basic validation rules (NotEmpty, format checks) first
3. **Keep validators focused** - One validator per request, or use `Include()` to compose
4. **Use meaningful error messages** - Help your API consumers understand what went wrong
5. **Register validators as Scoped** - Allows injecting scoped dependencies (e.g., DbContext)
6. **Test validators independently** - Unit test validators separately from handlers

```csharp
// Example: Testing validators
[Fact]
public void Should_Have_Error_When_Username_Is_Empty()
{
    var validator = new CreateUserCommandValidator();
    var command = new CreateUserCommand { Username = "" };
    
    var result = validator.Validate(command);
    
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.Username));
}
```

---

## 🧪 Running Tests Locally

```bash
dotnet test
```

The test suite needs a MediatR license key. MediatR resolves it from the environment
(`MEDIATR_LICENSE_KEY`, then the shared `LUCKYPENNY_LICENSE_KEY`) unless a key is passed explicitly
via `WithLicenseKey(...)`. In CI the value comes from the `MEDIATR_LICENSE_KEY` GitHub secret
(see `.github/workflows/ci.yml`).

To supply it locally, copy the template and fill in your key:

```bash
cp .env.example .env
```

```dotenv
MEDIATR_LICENSE_KEY=your-license-key
```

`.env` is git ignored, so the key never lands in the repository. A module initializer in the test
project (`TestEnvironment`) uses [dotenv.net](https://github.com/bolorundurowb/dotenv.net) to probe
upwards from the test output directory for the nearest `.env` and exports every `KEY=VALUE` entry
into the test process before the first mediator is resolved. Variables that are already set in the
environment are never overwritten, so CI values always win over the file.

This works the same for `dotnet test` and for the test runners in Visual Studio, Rider, and VS Code.
If you prefer not to keep a file in the working tree, set a user-level environment variable instead
(`setx MEDIATR_LICENSE_KEY "your-license-key"` on Windows) and restart your IDE or shell.

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Adam "AdaskoTheBeAsT" Pluciński**

- GitHub: [@AdaskoTheBeAsT](https://github.com/AdaskoTheBeAsT)
- NuGet: [AdaskoTheBeAsT packages](https://www.nuget.org/profiles/AdaskoTheBeAsT)

---

## ⭐ Show Your Support

If this library helps you, please give it a ⭐ on [GitHub](https://github.com/AdaskoTheBeAsT/AdaskoTheBeAsT.FluentValidation.MediatR)!
