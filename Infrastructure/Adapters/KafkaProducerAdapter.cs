using Confluent.Kafka;
using Core.Ports;
using Microsoft.Extensions.Logging;
using System;

namespace Infrastructure.Adapters
{
    public class KafkaProducerAdapter : IProducerPort, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerAdapter> _logger;

        public KafkaProducerAdapter(string bootstrapServers, ILogger<KafkaProducerAdapter> logger)
        {
            _logger = logger;
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public void SendMessage(string topic, string message)
        {
            try
            {
                _producer.Produce(topic, new Message<Null, string> { Value = message }, DeliveryHandler);
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError($"Delivery failed: {ex.Error.Reason}");
            }
        }

        private void DeliveryHandler(DeliveryReport<Null, string> report)
        {
            if (report.Error.IsError)
            {
                _logger.LogError($"Delivery Error: {report.Error.Reason}");
            }
            else
            {
                _logger.LogInformation($"Delivered message to {report.TopicPartitionOffset}");
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
    }
}
