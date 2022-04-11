using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace API.BusCommunication
{
    public class FilesUploadMessageProducer : IFilesUploadMessageProducer
    {
        private readonly IConfigurationRoot _configuration;
        private IConnection _connection;
        public FilesUploadMessageProducer(IConfigurationRoot configuration)
        {
            _configuration = configuration;

            CreateConnection();
        }

        public void SendMessage<T>(T message)
        {
            if (ConnectionExists())
            {
                var brokerConfig = _configuration.GetRequiredSection("MessageBroker");

                using var channel = _connection.CreateModel();
                var exchangeName = brokerConfig.GetValue<string>("ExchangeName");
                channel.QueueDeclare(exchangeName, false, false, false);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "", routingKey: exchangeName, null, body);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
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
    }
}
