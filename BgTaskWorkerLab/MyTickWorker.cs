namespace BgTaskWorkerLab
{
    public class MyTickWorker : BackgroundService
    {
        private readonly ILogger<MyTickWorker> _logger;

        public MyTickWorker(ILogger<MyTickWorker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Started at: {time:HH:mm:ss}", DateTime.Now);
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped at: {time:HH:mm:ss}", DateTime.Now);
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Running at: {time:HH:mm:ss}", DateTime.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}