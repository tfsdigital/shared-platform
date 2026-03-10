# Validations

FluentValidation extensions and common validators (CPF, CNPJ).

## Architecture

- `ICommandValidator<T>`: Validates commands before handler execution
- `CommandValidator<T>`: Wraps FluentValidation `IValidator<T>`
- `CpfValidator`, `CnpjValidator`: FluentValidation rule extensions

## Main Abstractions

```csharp
public interface ICommandValidator<in T>
{
    Result Validate(T command);
}

public class CommandValidator<T>(IValidator<T> validator) : ICommandValidator<T>
{
    public Result Validate(T command) => /* FluentValidation result → Result */;
}
```

## Usage Example

```csharp
public class CreateEstablishmentCommandValidator : AbstractValidator<CreateEstablishmentCommand>
{
    public CreateEstablishmentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Document).NotEmpty().Cnpj();  // From Validations
    }
}

// In handler (or pipeline)
var validation = _validator.Validate(command);
if (validation.IsFailure)
    return validation;
```

Use `.Cpf()` and `.Cnpj()` for Brazilian document validation in FluentValidation rules.
