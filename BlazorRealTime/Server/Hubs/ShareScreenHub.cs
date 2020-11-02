using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRealTime.Server.Services;
using BlazorRealTime.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;

namespace BlazorRealTime.Server.Hubs
{
    public class ShareScreenHub : Hub
    {
        private readonly CastManager _castManager;
        private const string AgentGroupPrefix = "AGENT_";
        public ShareScreenHub(CastManager castManager)
        {
            this._castManager = castManager;
        }
        public async Task AddScreenCastAgent(string agentName)
        {
            await Clients.Others.SendAsync("NewScreenCastAgent", agentName);
            await Groups.AddToGroupAsync(Context.ConnectionId, AgentGroupPrefix + agentName);
            _castManager.MessageCount += 2;
        }

        public async Task RemoveScreenCastAgent(string agentName)
        {
            await Clients.Others.SendAsync("RemoveScreenCastAgent", agentName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AgentGroupPrefix + agentName);
            _castManager.RemoveViewerByAgent(agentName);
            _castManager.MessageCount += 2;
        }

        public async Task AddScreenCastViewer(string agentName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, agentName);
            _castManager.AddViewer(Context.ConnectionId, agentName);
            await Clients.Groups(AgentGroupPrefix + agentName).SendAsync("NewViewer");
            _castManager.MessageCount += 2;
        }

        public async Task RemoveScreenCastViewer(string agentName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, agentName);
            _castManager.RemoveViewer(Context.ConnectionId);
            if (!_castManager.IsViewerExists(agentName))
            {
                await Clients.Groups(AgentGroupPrefix + agentName).SendAsync("NoViewer");
                _castManager.MessageCount += 1;
            }
            _castManager.MessageCount += 1;
        }
        public async Task StreamCastData(IAsyncEnumerable<string> stream, string agentName)
        {
            _castManager.MessageCount += 1;
            await foreach (var item in stream)
            {
                await Clients.Group(agentName).SendAsync("OnStreamCastDataReceived", item);
                _castManager.MessageCount += 1;
            }
        }

        public async Task SendChat(string agentName, ChatMessage message)
        {
            await Clients.Group(agentName).SendAsync("ReceiveMessage", message);
            await Clients.Group(AgentGroupPrefix + agentName).SendAsync("ReceiveMessage", message);
            _castManager.MessageCount += 2;
        }
    }
}
