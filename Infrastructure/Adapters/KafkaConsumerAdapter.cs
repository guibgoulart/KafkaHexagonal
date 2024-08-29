using Confluent.Kafka;
using Core.Ports;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Adapters
{
    public class KafkaConsumerAdapter : IConsumerPort, IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ILogger<KafkaConsumerAdapter> _logger;
        private Task _consumerTask;
        private CancellationTokenSource _cts;

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
            if (_consumerTask != null && !_consumerTask.IsCompleted)
            {
                _logger.LogWarning("Consumer is already running.");
                return;
            }

            _cts = new CancellationTokenSource();
            _consumerTask = Task.Run(() => StartConsuming(topic, _cts.Token), _cts.Token);
        }

        private void StartConsuming(string topic, CancellationToken cancellationToken)
        {
            _consumer.Subscribe(topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var cr = _consumer.Consume(cancellationToken);
                        // Put logic here
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
                _logger.LogInformation("Consumption canceled.");
            }
            finally
            {
                _consumer.Close();
            }
        }

        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _consumerTask.Wait();
                _cts.Dispose();
                _logger.LogInformation("Consumer stopped.");
            }
        }

        public void Dispose()
        {
            Stop();
            _consumer.Dispose();
        }
    }
}
