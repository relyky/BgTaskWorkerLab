using BgTaskWorkerLab;
using BgTaskWorkerLab.Model;
using BgTaskWorkerLab.Services;

/// <see cref="[Background tasks with hosted services in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio)"/>

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostCtx, services) =>
    {
        //## 註冊可注入的服務
        services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
        services.AddSingleton<IBackgroundTaskQueue>(ctx =>
        {
            int queueCapacity = 100;
            int.TryParse(hostCtx.Configuration["QueueCapacity"], out queueCapacity);
            return new BackgroundTaskQueue(queueCapacity);
        });

        //## 註冊 Worker Service 
        //# DEMO 1 2 3 --- 可關閉這些展示以簡化輸出
        services.AddHostedService<MyTickWorker>();
        services.AddHostedService<MyTimerHostedWorker>();
        services.AddHostedService<MyConsumeScopedServiceWorker>();

        //# DEMO 4
        // 更接進真實的應用：用來生產 taskQueue
        services.AddHostedService<MyMonitorWorkder>();
        // 更接進真實的應用：用來消費 taskQueue
        services.AddHostedService<MyQueuedWorker>();

    })
    .Build();

// 一般啟動
await host.RunAsync();

////## 進階啟動分二段指令
//
//// 進階的啟動指令段一。
//await host.StartAsync();
//
//// 經由注入建構 MonitorLoop
//var monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
//monitorLoop.StartMonitorLoop();
//
//// 進階的啟動指令段二。
//await host.WaitForShutdownAsync();
