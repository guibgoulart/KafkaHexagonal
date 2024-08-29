using Core.Entities;
using Core.Ports;
using Infrastructure.Adapters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsumerController : ControllerBase
    {
        private readonly IConsumerPort _consumer;
        private readonly ILogger<ConsumerController> _logger;

        public ConsumerController(IConsumerPort kafkaConsumer, ILogger<ConsumerController> logger)
        {
            _consumer = kafkaConsumer;
            _logger = logger;
        }

        [HttpPost("consume")]
        public IActionResult StartConsuming([FromBody] ConsumeRequest request)
        {
            _logger.LogInformation($"Starting to consume topic: {request.Topic}");
            _consumer.Consume(request.Topic);
            return Ok();
        }

        [HttpPost("stop")]
        public IActionResult StopConsuming()
        {
            _logger.LogInformation("Stopping consumer.");
            if (_consumer is KafkaConsumerAdapter consumerAdapter)
            {
                consumerAdapter.Stop();
            }
            return Ok();
        }
    }
}
