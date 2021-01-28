using RabbitMQ.Client;
using System;
using System.Text;

namespace Topics.Producer
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
          channel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: "topic", durable: true, autoDelete: false);

          const string routingKey = "staging.payment.authorization";

          // TODO: try to send an actual hasura payload!
          var message = "hasura payload";
          var body = Encoding.UTF8.GetBytes(message);

          channel.BasicPublish(exchange: EXCHANGE_NAME, routingKey: routingKey, basicProperties: null, body: body);

          Console.WriteLine("Sent {0}", message);
        }
      }
    }
  }
}
