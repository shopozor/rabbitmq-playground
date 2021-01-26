using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Consumer
{
  class Program
  {
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
          channel.QueueDeclare(queue: "task_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

          var consumer = new EventingBasicConsumer(channel);
          consumer.Received += (sender, ea) =>
          {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received {0}", message);

            var dots = message.Split('.').Length - 1;
            Thread.Sleep(dots * 1000);

            Console.WriteLine("Done");

            // Note: it is possible to access the channel via
            //       ((EventingBasicConsumer)sender).Model here
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
          };

          channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);

          Console.WriteLine("Press [enter] to exit");
          Console.ReadLine();
        }
      }
    }
  }
}
