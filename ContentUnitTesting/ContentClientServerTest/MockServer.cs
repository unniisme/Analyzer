using System;
using Networking.Communicator;
using Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentUnitTesting.ContentClientServer
{
    public class MockServer : ICommunicator
    {
        public IEventHandler _eventHandler;
        public void Send(string serializedObj, string eventType, string destID)
        {
            throw new NotImplementedException();
        }

        public IEventHandler GetEvents()
        {
            return _eventHandler;
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
            _eventHandler = eventHandler;
        }
    }
}
