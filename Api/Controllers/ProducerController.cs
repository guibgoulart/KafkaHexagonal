using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly MessagingService _messagingService;

        public ProducerController(MessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        [HttpPost]
        public IActionResult PublishMessage([FromBody] string message)
        {
            _messagingService.PublishMessage("test-topic", message);
            return Ok();
        }
    }
}