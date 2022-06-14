using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServer
{
    public interface ILogProducer
    {
        void PublishMessage(string message);
    }

    public class LogProducer : ILogProducer
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        public LogProducer()
        {
            this.factory = new ConnectionFactory() { HostName = "localhost" };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
            DeclareQueue(this.channel);
        }

        public void DeclareQueue(IModel channel)
        {
            channel.QueueDeclare(queue: "log_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );
        }
        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                routingKey: "log_queue",
                basicProperties: null,
                body: body);
        }
    }
}