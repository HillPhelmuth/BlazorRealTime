using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRealTime.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorRealTime.Client.Pages
{
    public partial class ViewSignalR : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Parameter] 
        public string ViewerName { get; set; } = "anonymous";
        private List<string> agents = new List<string>();

        private HubConnection connection;
        private string imageSource = null;
        private string CurrentViewCastAgent = null;
        private List<ChatMessage> messages = new List<ChatMessage>();

        protected override async Task OnInitializedAsync()
        {
            connection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/shareScreen"))
                .Build();

            connection.On<string>("NewScreenCastAgent", NewScreenCastAgent);
            connection.On<string>("RemoveScreenCastAgent", RemoveScreenCastAgent);
            connection.On<string>("OnStreamCastDataReceived", OnStreamCastDataReceived);
            connection.On<ChatMessage>("ReceiveMessage", OnMessageReceived);

            await connection.StartAsync();
        }

        private bool IsViewingCastOf(string agentName)
        {
            return agentName == CurrentViewCastAgent;
        }

        private void NewScreenCastAgent(string agentName)
        {
            agents.Add(agentName);
            StateHasChanged();
        }

        private void RemoveScreenCastAgent(string agentName)
        {
            agents.Remove(agentName);
            imageSource = null;
            CurrentViewCastAgent = null;
            StateHasChanged();
        }

        private void OnStreamCastDataReceived(string streamData)
        {
            imageSource = streamData;
            StateHasChanged();
        }

        private void OnMessageReceived(ChatMessage message)
        {
            messages.Add(message);
            if (messages.Count > 20)
                messages.RemoveAt(0);
            StateHasChanged();
        }
        private async Task SendChat(string message = "Empty-Chat")
        {
            await connection.SendAsync("SendChat", CurrentViewCastAgent, new ChatMessage(ViewerName, message));
        }
        private async Task OnViewCastClicked(string agentName)
        {
            CurrentViewCastAgent = agentName;
            await connection.InvokeAsync("AddScreenCastViewer", agentName);
        }

        private async Task OnStopViewCastClicked(string agentName)
        {
            CurrentViewCastAgent = null;
            await connection.InvokeAsync("RemoveScreenCastViewer", agentName);
            imageSource = null;
            StateHasChanged();
        }
    }
}
