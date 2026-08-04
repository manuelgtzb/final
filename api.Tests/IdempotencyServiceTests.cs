using LandingFinal.Api.Services;

namespace LandingFinal.Api.Tests;

public sealed class IdempotencyServiceTests
{
    [Fact]
    public async Task ExecuteAsync_CoalescesConcurrentDuplicateRequests()
    {
        var service = new IdempotencyService();
        var key = Guid.NewGuid();
        var calls = 0;

        async Task<ContactOperationResult> Operation()
        {
            Interlocked.Increment(ref calls);
            await Task.Delay(30);
            return new ContactOperationResult(200, "sent", "ok", "email-id");
        }

        var results = await Task.WhenAll(
            service.ExecuteAsync(key, "same-fingerprint", Operation),
            service.ExecuteAsync(key, "same-fingerprint", Operation));

        Assert.Equal(1, calls);
        Assert.All(results, result => Assert.Equal(200, result.Result.StatusCode));
        Assert.Single(results, result => !result.IsDuplicate);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsTheSameKeyForDifferentData()
    {
        var service = new IdempotencyService();
        var key = Guid.NewGuid();
        static Task<ContactOperationResult> Operation() =>
            Task.FromResult(new ContactOperationResult(200, "sent", "ok"));

        await service.ExecuteAsync(key, "first", Operation);
        var conflict = await service.ExecuteAsync(key, "second", Operation);

        Assert.True(conflict.IsConflict);
        Assert.Equal(400, conflict.Result.StatusCode);
    }
}
