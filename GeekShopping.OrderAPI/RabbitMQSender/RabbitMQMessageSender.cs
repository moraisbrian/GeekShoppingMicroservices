using System.Text;
using System.Text.Json;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.MessageBus;
using RabbitMQ.Client;

namespace GeekShopping.OrderAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _userName;
        private IConnection _connection;

        public RabbitMQMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
        }

        public void SendMessage(BaseMessage message, string queueName)
        {
            if (ConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, false, false, false, null);
                    var body = GetMessageAsByteArray(message);
                    channel.BasicPublish("", queueName, basicProperties: null, body: body);
                } 
            }
        }

        private byte[] GetMessageAsByteArray(BaseMessage message)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize<PaymentDTO>((PaymentDTO)message, options);
            return Encoding.UTF8.GetBytes(json);
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
                return true;

            CreateConnection();

            return _connection != null;
        }
    }
}