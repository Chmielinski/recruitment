using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Worker.Services.Interfaces;

namespace Worker.BusCommunication
{
    public class FileUploadMessageReceiver : BackgroundService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IVisitorsService _visitorsService;
        private readonly ILogger<FileUploadMessageReceiver> _logger;
        private IConnection _connection;
        private IModel _channel;
        private string _exchangeName;

        public FileUploadMessageReceiver(IConfigurationRoot configuration, IVisitorsService visitorsService, ILogger<FileUploadMessageReceiver> logger)
        {
            _configuration = configuration;
            _visitorsService = visitorsService;
            _logger = logger;

            var brokerConfig = _configuration.GetRequiredSection("MessageBroker");
            _exchangeName = brokerConfig.GetValue<string>("ExchangeName");

            InitializeListener();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if (ConnectionExists())
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, e) =>
                {
                    try
                    {
                        Console.WriteLine($"Consuming event data.");

                        await _visitorsService.ProcessFiles();

                        _channel.BasicAck(e.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                };

                _channel.BasicConsume(_exchangeName, false, consumer);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private void InitializeListener()
        {
            if (ConnectionExists())
            {
                _channel = _connection.CreateModel();

                Console.WriteLine("Declared queue.");

                _channel.QueueDeclare(_exchangeName, false, false, false);
            }
        }

        private void CreateConnection()
        {
            try
            {
                var brokerConfig = _configuration.GetRequiredSection("MessageBroker");

                var factory = new ConnectionFactory
                {
                    HostName = brokerConfig.GetValue<string>("HostName"),
                    UserName = brokerConfig.GetValue<string>("UserName"),
                    Password = brokerConfig.GetValue<string>("Password"),
                    Port = brokerConfig.GetValue<int>("Port"),
                };

                _connection = factory.CreateConnection();

                Console.WriteLine($"Connection created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not establish connection - {ex.Message}.");
                throw;
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
