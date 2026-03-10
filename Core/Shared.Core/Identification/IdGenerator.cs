namespace Shared.Core.Identification;

public static class IdGenerator
{
    public static Guid CreateSequential() => Guid.CreateVersion7();
    public static Guid Create() => Guid.NewGuid();
}
