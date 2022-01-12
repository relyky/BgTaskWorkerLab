namespace BgTaskWorkerLab;

/// <summary>
/// 最基礎的 Backgournd Worker Service 用法。
/// </summary>
public class MyTimerHostedWorker : IHostedService, IDisposable
{
    //## Injection Member
    readonly ILogger<MyTimerHostedWorker> _logger;

    //## Resource
    private Timer _timer = null!;

    //## State
    private int executionCount = 0;

    public MyTimerHostedWorker(ILogger<MyTimerHostedWorker> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timer start at: {time:HH:mm:ss}", DateTime.Now);
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timer stop at: {time:HH:mm:ss}", DateTime.Now);
        _timer?.Change(Timeout.Infinite, 0); // 等同 disable timer。 
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var count = Interlocked.Increment(ref executionCount);
        _logger.LogInformation($"Timer DoWork {count} times.", count);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}