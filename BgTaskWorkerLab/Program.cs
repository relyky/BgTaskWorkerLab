using BgTaskWorkerLab;
using BgTaskWorkerLab.Services;

/// <see cref="[Background tasks with hosted services in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio)"/>

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

        //�� HostedServic ���O singleton�C
        services.AddHostedService<MyTickWorker>();
        services.AddHostedService<MyTimerHostedWorker>();
        services.AddHostedService<MyConsumeScopedServiceHostedService>();
    })
    .Build();

await host.RunAsync();
