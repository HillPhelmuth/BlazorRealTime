using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorRealTime.Server.Services
{
    public class CastManager
    {
        private readonly List<Viewer> _viewers = new List<Viewer>();
        private int _messageCount;

        public int MessageCount
        {
            get => _messageCount;
            set
            {
                _messageCount = value;
                Console.WriteLine($"Message count = {_messageCount}");
            }
        }

        public void AddViewer(string connectionId, string agentName)
        {
            _viewers.Add(new Viewer(connectionId, agentName));
        }

        public void RemoveViewer(string connectionId)
        {
            _viewers.Remove(_viewers.First(i => i.ConnectionId == connectionId));
        }

        public void RemoveViewerByAgent(string agentName)
        {
            _viewers.RemoveAll(i => i.AgentName == agentName);
        }

        public bool IsViewerExists(string agentName)
        {
            return _viewers.Any(i => i.AgentName == agentName);
        }

    }

    internal class Viewer
    {
        public string ConnectionId { get; set; }
        public string AgentName { get; set; }

        public Viewer(string connectionId, string agentName)
        {
            ConnectionId = connectionId;
            AgentName = agentName;
        }
    }
}
