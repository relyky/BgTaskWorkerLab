//#define DEMO_123
#define DEMO_4

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

        // 用來生產 taskQueue
        services.AddSingleton<MonitorLoop>();

        //## 註冊 Worker Service 
#if DEMO_123
        services.AddHostedService<MyTickWorker>();
        services.AddHostedService<MyTimerHostedWorker>();
        services.AddHostedService<MyConsumeScopedServiceWorker>();
#endif
#if DEMO_4
        //// 更接進真實的應用：用來生產 taskQueue
        //services.AddHostedService<JobScheduleMonitorLoop>();

        // 更接進真實的應用：用來消費 taskQueue
        services.AddHostedService<MyQueuedWorker>();
#endif
    })
    .Build();

#if DEMO_123
//## 一般啟動只需執行 RunAsync() 即可。
await host.RunAsync();
#endif

#if DEMO_4
//## 進階的啟動分二段指令。

// 進階的啟動指令段一。
await host.StartAsync();

// 經由注入建構 MonitorLoop
var monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
monitorLoop.StartMonitorLoop();

// 進階的啟動指令段二。
await host.WaitForShutdownAsync();

#endif 