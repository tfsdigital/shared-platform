# Shared Libraries

Cross-cutting libraries used across bounded contexts (establishments, orders, products, notifications).

| Feature                                           | Description                                  |
| ------------------------------------------------- | -------------------------------------------- |
| [Caching](libs/Caching/README.md)                 | Cache abstraction and Redis implementation   |
| [Contracts](libs/Contracts/README.md)             | Cross-context integration events             |
| [Core](libs/Core/README.md)                       | Domain base classes, events, identification  |
| [Correlation](libs/Correlation/README.md)         | Correlation ID for distributed tracing       |
| [ExternalClients](libs/ExternalClients/README.md) | HTTP client abstractions and implementations |
| [Handlers](libs/Handlers/README.md)               | CQRS handler interfaces and base classes     |
| [Identity](libs/Identity/README.md)               | JWT, claims, and authorization helpers       |
| [Inbox](libs/Inbox/README.md)                     | Idempotent event consumption                 |
| [Messaging](libs/Messaging/README.md)             | Message bus abstraction (RabbitMQ)           |
| [Outbox](libs/Outbox/README.md)                   | Reliable event publishing                    |
| [Publishing](libs/Publishing/README.md)           | Domain event publishing                      |
| [Queries](libs/Queries/README.md)                 | Pagination and query models                  |
| [Results](libs/Results/README.md)                 | Result pattern for error handling            |
| [Validations](libs/Validations/README.md)         | FluentValidation extensions and validators   |

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

## GitHub Packages Setup

To publish NuGet packages to GitHub Packages with the existing release workflow, make sure the repository is hosted under the `devfactorylabs` owner and GitHub Actions is allowed to publish packages for this repository.

The workflow expects:

- a tag in the format `vX.Y.Z`
- `GITHUB_TOKEN` with `packages: write` permission
- `nuget.config` committed with the `github` source

After the first successful release, packages will be available at:

```text
https://github.com/orgs/devfactorylabs/packages?repo_name=shared-platform
```

To consume the packages locally, authenticate a NuGet source with a GitHub Personal Access Token that has at least `read:packages`:

```bash
dotnet nuget update source github \
  --source "https://nuget.pkg.github.com/devfactorylabs/index.json" \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_PAT \
  --store-password-in-clear-text
```
