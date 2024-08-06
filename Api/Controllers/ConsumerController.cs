using Core.Ports;
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
        public IActionResult StartConsuming([FromBody] string topic)
        {
            _logger.LogInformation($"Starting to consume topic: {topic}");
            _consumer.Consume(topic);
            return Ok();
        }
    }
}
