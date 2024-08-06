using Core.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class MessagingService
    {
        private readonly IProducerPort _messagingPort;

        public MessagingService(IProducerPort messagingPort)
        {
            _messagingPort = messagingPort;
        }

        public void PublishMessage(string topic, string message)
        {
            _messagingPort.SendMessage(topic, message);
        }
    }
}

