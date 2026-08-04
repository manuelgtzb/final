using System.Collections.Concurrent;

namespace LandingFinal.Api.Services;

public sealed record ContactOperationResult(int StatusCode, string Code, string Message, string? EmailId = null);

public sealed record IdempotencyExecutionResult(ContactOperationResult Result, bool IsDuplicate, bool IsConflict = false);

public sealed class IdempotencyService
{
    private static readonly TimeSpan Retention = TimeSpan.FromHours(24);
    private readonly ConcurrentDictionary<Guid, Entry> entries = new();

    public async Task<IdempotencyExecutionResult> ExecuteAsync(
        Guid key,
        string fingerprint,
        Func<Task<ContactOperationResult>> operation)
    {
        while (true)
        {
            if (entries.TryGetValue(key, out var existing))
            {
                if (existing.ExpiresAt <= DateTimeOffset.UtcNow)
                {
                    entries.TryRemove(new KeyValuePair<Guid, Entry>(key, existing));
                    continue;
                }

                if (!string.Equals(existing.Fingerprint, fingerprint, StringComparison.Ordinal))
                {
                    return new IdempotencyExecutionResult(
                        new ContactOperationResult(400, "idempotency_conflict", "The idempotency key was already used for different data."),
                        IsDuplicate: false,
                        IsConflict: true);
                }

                return new IdempotencyExecutionResult(await existing.Operation.Value, IsDuplicate: true);
            }

            var entry = new Entry(
                fingerprint,
                new Lazy<Task<ContactOperationResult>>(operation, LazyThreadSafetyMode.ExecutionAndPublication),
                DateTimeOffset.UtcNow.Add(Retention));

            if (!entries.TryAdd(key, entry))
            {
                continue;
            }

            try
            {
                var result = await entry.Operation.Value;
                if (result.StatusCode != 200)
                {
                    entries.TryRemove(new KeyValuePair<Guid, Entry>(key, entry));
                }

                return new IdempotencyExecutionResult(result, IsDuplicate: false);
            }
            catch
            {
                entries.TryRemove(new KeyValuePair<Guid, Entry>(key, entry));
                throw;
            }
        }
    }

    private sealed record Entry(
        string Fingerprint,
        Lazy<Task<ContactOperationResult>> Operation,
        DateTimeOffset ExpiresAt);
}
