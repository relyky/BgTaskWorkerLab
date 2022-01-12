using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BgTaskWorkerLab.Services;

namespace BgTaskWorkerLab;

internal class MyConsumeScopedServiceHostedService : BackgroundService
{
    readonly ILogger<MyConsumeScopedServiceHostedService> _logger;
    readonly IServiceProvider _provider;

    public MyConsumeScopedServiceHostedService(
        IServiceProvider provider,
        ILogger<MyConsumeScopedServiceHostedService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service running.");
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service is working.");

        //※注意：需繞一圈來注入(injection) scope 服務物件。
        /// 因為 BackgroundService 是 singleton 服務物件而 scope 與 singleton 這兩者不能一起注入。
        using var scope = _provider.CreateScope();
        // 注入 scope 服務物件
        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

        //# 終於開始執行工作
        await scopedProcessingService.DoWork(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}