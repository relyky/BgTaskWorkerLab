using BgTaskWorkerLab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BgTaskWorkerLab;

/// <summary>
/// 為 TaskQueue 之生產者
/// </summary>
internal class MyMonitorWorkder : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly CancellationToken _cancellationToken;

    public MyMonitorWorkder(
        ILogger<MyMonitorWorkder> logger,
        IBackgroundTaskQueue taskQueue,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _taskQueue = taskQueue;
        _cancellationToken = applicationLifetime.ApplicationStopping;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MonitorAsync Loop is starting.");

        /// 正式應用：監看排程中的 Job 若安排的時間已到則放入 taskQueue 後續再依次消化。
        await Task.Run(async () => await MonitorAsync());
    }

    private async ValueTask MonitorAsync()
    {
        int launchCount = 0;

        while (!_cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("監看排程中有否工作要執行...");

            /// 正式應用：監看排程中的 Job 若安排的時間已到則放入 taskQueue 後續再依次消化。

            /// 模擬應用：每7秒發出一個 task 發出3次就結束。
            if (launchCount < 3)
            {
                // Enqueue a background work item
                await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem);
                launchCount++;
            }

            // wait a moment
            await Task.Delay(7000);
        }
    }

    private async ValueTask BuildWorkItem(CancellationToken token)
    {
        // Simulate three 5-second tasks to complete
        // for each enqueued work item

        int delayLoop = 0;
        var guid = Guid.NewGuid().ToString();

        _logger.LogInformation("Queued Background Task {Guid} is starting.", guid);

        while (!token.IsCancellationRequested && delayLoop < 3)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if the Delay is cancelled
            }

            delayLoop++;

            _logger.LogInformation("Queued Background Task {Guid} is running. "
                                   + "{DelayLoop}/3", guid, delayLoop);
        }

        if (delayLoop == 3)
        {
            _logger.LogInformation("Queued Background Task {Guid} is complete.", guid);
        }
        else
        {
            _logger.LogInformation("Queued Background Task {Guid} was cancelled.", guid);
        }
    }
}
