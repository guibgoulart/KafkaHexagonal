using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Confluent.Kafka;
using Core.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Infrastructure.Adapters
{
    public class KafkaConsumerAdapter : IConsumerPort
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ILogger<KafkaConsumerAdapter> _logger;

        public KafkaConsumerAdapter(string bootstrapServers, string groupId, ILogger<KafkaConsumerAdapter> logger)
        {
            _logger = logger;
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public void Consume(string topic)
        {
            _consumer.Subscribe(topic);

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = _consumer.Consume(cts.Token);
                        _logger.LogInformation($"Consumed message '{cr.Value}' from topic {cr.Topic}, partition {cr.Partition}, offset {cr.Offset}");
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Error occurred: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
            }
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}

