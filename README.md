# Shared Libraries

Cross-cutting libraries used across bounded contexts (establishments, orders, products, notifications).

| Feature | Description |
|---------|-------------|
| [Caching](Caching/README.md) | Cache abstraction and Redis implementation |
| [Contracts](Contracts/README.md) | Cross-context integration events |
| [Core](Core/README.md) | Domain base classes, events, identification |
| [Correlation](Correlation/README.md) | Correlation ID for distributed tracing |
| [ExternalClients](ExternalClients/README.md) | HTTP client abstractions and implementations |
| [Handlers](Handlers/README.md) | CQRS handler interfaces and base classes |
| [Identity](Identity/README.md) | JWT, claims, and authorization helpers |
| [Inbox](Inbox/README.md) | Idempotent event consumption |
| [Messaging](Messaging/README.md) | Message bus abstraction (RabbitMQ) |
| [Outbox](Outbox/README.md) | Reliable event publishing |
| [Publishing](Publishing/README.md) | Domain event publishing |
| [Queries](Queries/README.md) | Pagination and query models |
| [Results](Results/README.md) | Result pattern for error handling |
| [Validations](Validations/README.md) | FluentValidation extensions and validators |

## Development

### Build

```bash
dotnet build shared-platform.slnx
```

### Test

```bash
dotnet test shared-platform.slnx
```

### Code Coverage

```bash
chmod +x coverage.sh
./coverage.sh
# Opens coverage-report/index.html with the full report
```

### Publishing a New Version

Versioning follows [Semantic Versioning](https://semver.org/) (major.minor.patch) via git tags:

```bash
git tag v1.2.3
git push origin v1.2.3
```

This triggers the release workflow, which builds, tests, and publishes all packages to GitHub Packages automatically.
