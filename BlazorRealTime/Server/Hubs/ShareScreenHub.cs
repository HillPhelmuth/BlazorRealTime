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
        private readonly ScreenCastManager screenCastManager;
        private const string AGENT_GROUP_PREFIX = "AGENT_";
        public ShareScreenHub(ScreenCastManager screenCastManager)
        {
            this.screenCastManager = screenCastManager;
        }
        public async Task AddScreenCastAgent(string agentName)
        {
            await Clients.Others.SendAsync("NewScreenCastAgent", agentName);
            await Groups.AddToGroupAsync(Context.ConnectionId, AGENT_GROUP_PREFIX + agentName);
        }

        public async Task RemoveScreenCastAgent(string agentName)
        {
            await Clients.Others.SendAsync("RemoveScreenCastAgent", agentName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AGENT_GROUP_PREFIX + agentName);
            screenCastManager.RemoveViewerByAgent(agentName);
        }

        public async Task AddScreenCastViewer(string agentName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, agentName);
            screenCastManager.AddViewer(Context.ConnectionId, agentName);
            await Clients.Groups(AGENT_GROUP_PREFIX + agentName).SendAsync("NewViewer");
        }

        public async Task RemoveScreenCastViewer(string agentName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, agentName);
            screenCastManager.RemoveViewer(Context.ConnectionId);
            if (!screenCastManager.IsViewerExists(agentName))
                await Clients.Groups(AGENT_GROUP_PREFIX + agentName).SendAsync("NoViewer");
        }
        public async Task StreamCastData(IAsyncEnumerable<string> stream, string agentName)
        {
            await foreach (var item in stream)
            {
                await Clients.Group(agentName).SendAsync("OnStreamCastDataReceived", item);
            }
        }
        public async Task Join(string user)
        {
            await Clients.All.SendAsync("Joined", user);
        }
        public async Task SendElement(string data)
        {
            //var items = Convert.FromBase64String(data);
            Console.WriteLine($"SendElement receieved {data.Substring(0, 20)}... Sending to DrawFrame");
            await Clients.All.SendAsync("DrawFrame", data);
        }

        public async Task StreamData(IAsyncEnumerable<ScreenDataItem> stream)
        {
            await foreach (var item in stream)
            {
                await Task.Delay(10);
                await Clients.All.SendAsync("HandleStreamData", item);
            }
        }
    }
}
