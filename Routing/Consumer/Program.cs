using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Routing.Consumer
{
  class Program
  {
    private const string EXCHANGE_NAME = "openfaas_staging";

    static void Main(string[] args)
    {
      var factory = new ConnectionFactory
      {
        HostName = "localhost",
        UserName = "admin",
        Password = "password"
      };
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          channel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: "direct", durable: true, autoDelete: false);
          var queueName = channel.QueueDeclare().QueueName;
          const string routingKey = "staging.payment.authorization";
          channel.QueueBind(queue: queueName, exchange: EXCHANGE_NAME, routingKey: routingKey);

          Console.WriteLine("Waiting for messages...");

          var consumer = new EventingBasicConsumer(channel);
          consumer.Received += (sender, ea) =>
          {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received {0}", message);
          };

          channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

          Console.WriteLine("Press [enter] to exit");
          Console.ReadLine();
        }
      }
    }
  }
}
