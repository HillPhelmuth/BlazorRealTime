using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorRealTime.Client.Pages
{
    public partial class ViewSignalR
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private List<string> agents = new List<string>();

        HubConnection connection;
        string imageSource = null;
        string CurrentViewCastAgent = null;

        protected override async Task OnInitializedAsync()
        {
            connection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/shareScreen"))
                .Build();

            connection.On<string>("NewScreenCastAgent", NewScreenCastAgent);
            connection.On<string>("RemoveScreenCastAgent", RemoveScreenCastAgent);
            connection.On<string>("OnStreamCastDataReceived", OnStreamCastDataReceived);

            await connection.StartAsync();
        }

        bool IsViewingCastOf(string agentName)
        {
            return agentName == CurrentViewCastAgent;
        }

        void NewScreenCastAgent(string agentName)
        {
            agents.Add(agentName);
            StateHasChanged();
        }

        void RemoveScreenCastAgent(string agentName)
        {
            agents.Remove(agentName);
            imageSource = null;
            CurrentViewCastAgent = null;
            StateHasChanged();
        }

        void OnStreamCastDataReceived(string streamData)
        {
            imageSource = streamData;
            StateHasChanged();
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
