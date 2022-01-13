//#define DEMO_123
#define DEMO_4

using BgTaskWorkerLab;
using BgTaskWorkerLab.Model;
using BgTaskWorkerLab.Services;

/// <see cref="[Background tasks with hosted services in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio)"/>

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostCtx, services) =>
    {
        //## ���U�i�`�J���A��
        services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
        services.AddSingleton<IBackgroundTaskQueue>(ctx =>
        {
            int queueCapacity = 100;
            int.TryParse(hostCtx.Configuration["QueueCapacity"], out queueCapacity);
            return new BackgroundTaskQueue(queueCapacity);
        });

        // �ΨӥͲ� taskQueue
        services.AddSingleton<MonitorLoop>();

        //## ���U Worker Service 
#if DEMO_123
        services.AddHostedService<MyTickWorker>();
        services.AddHostedService<MyTimerHostedWorker>();
        services.AddHostedService<MyConsumeScopedServiceWorker>();
#endif
#if DEMO_4
        //// �󱵶i�u�ꪺ���ΡG�ΨӥͲ� taskQueue
        //services.AddHostedService<JobScheduleMonitorLoop>();

        // �󱵶i�u�ꪺ���ΡG�ΨӮ��O taskQueue
        services.AddHostedService<MyQueuedWorker>();
#endif
    })
    .Build();

#if DEMO_123
//## �@��Ұʥu�ݰ��� RunAsync() �Y�i�C
await host.RunAsync();
#endif

#if DEMO_4
//## �i�����Ұʤ��G�q���O�C

// �i�����Ұʫ��O�q�@�C
await host.StartAsync();

// �g�Ѫ`�J�غc MonitorLoop
var monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
monitorLoop.StartMonitorLoop();

// �i�����Ұʫ��O�q�G�C
await host.WaitForShutdownAsync();

#endif 