﻿using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Models;
using GeekShopping.OrderAPI.RabbitMQSender;
using GeekShopping.OrderAPI.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private IConnection _connection;
        private IModel _channel;
        private const string ExchangeName = "FanoutPaymentUpdateExchange";
        private string queueName = "";

        public RabbitMQPaymentConsumer(OrderRepository repository)
        {
            _repository = repository;

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            //_channel.QueueDeclare("orderpaymentresultqueue", false, false, false, null);

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
            queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, ExchangeName, "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                var dto = JsonSerializer.Deserialize<UpdatePaymentResultDTO>(content);
                UpdatePaymentStatus(dto).GetAwaiter().GetResult();
                _channel.BasicAck(evt.DeliveryTag, false);
            };

            //_channel.BasicConsume("orderpaymentresultqueue", false, consumer);
            _channel.BasicConsume(queueName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task UpdatePaymentStatus(UpdatePaymentResultDTO dto)
        {
            try
            {
                await _repository.UpdateOrderPaymentStatus(dto.OrderId, dto.Status);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}