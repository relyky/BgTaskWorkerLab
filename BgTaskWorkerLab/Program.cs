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

        //## ���U Worker Service 
        //# DEMO 1 2 3 --- �i�����o�Ǯi�ܥH²�ƿ�X
        services.AddHostedService<MyTickWorker>();
        services.AddHostedService<MyTimerHostedWorker>();
        services.AddHostedService<MyConsumeScopedServiceWorker>();

        //# DEMO 4
        // �󱵶i�u�ꪺ���ΡG�ΨӥͲ� taskQueue
        services.AddHostedService<MyMonitorWorkder>();
        // �󱵶i�u�ꪺ���ΡG�ΨӮ��O taskQueue
        services.AddHostedService<MyQueuedWorker>();

    })
    .Build();

// �@��Ұ�
await host.RunAsync();

////## �i���Ұʤ��G�q���O
//
//// �i�����Ұʫ��O�q�@�C
//await host.StartAsync();
//
//// �g�Ѫ`�J�غc MonitorLoop
//var monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
//monitorLoop.StartMonitorLoop();
//
//// �i�����Ұʫ��O�q�G�C
//await host.WaitForShutdownAsync();
