using HasuraHandling.Data;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Routing.Producer
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

          const string routingKey = "staging.payment.authorization";

          var input = new Input
          {
            Transaction = new AuthorizedTransaction
            {
              Id = 59843,
              OrderId = Guid.NewGuid()
            }
          };
          var payload = new ActionRequestPayload<Input>
          {
            Action = new HasuraAction
            {
              Name = "my_hasura_action_name"
            },
            Input = input,
            SessionVariables = new HasuraSessionVariables
            {
              Role = "rex",
              UserId = Guid.NewGuid()
            }
          };
          var message = JsonConvert.SerializeObject(payload);
          var body = Encoding.Default.GetBytes(message);

          var props = channel.CreateBasicProperties();
          props.ContentType = "application/json";

          channel.BasicPublish(exchange: EXCHANGE_NAME, routingKey: routingKey, basicProperties: props, body: body);

          Console.WriteLine("Sent {0}", message);
        }
      }
    }
  }
}
