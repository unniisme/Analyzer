using System;
using Networking.Communicator;
using Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking.Utils;

namespace ContentUnitTesting.ContentClientServer
{
    public class MockClient : ICommunicator
    {
        private MockServer _server;
        MockClient(MockServer server)
        {
            _server = server;
        }

        public void Send(string serializedObj, string eventType, string destID)
        {
            Message message = new Message(serializedObj, eventType, destID, "");
            _server.GetEvents().HandleFile(message);
        }

        public string Start(string? destIP, int? destPort, string senderID)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(IEventHandler eventHandler, string moduleName)
        {
            throw new NotImplementedException();
        }
    }
}
