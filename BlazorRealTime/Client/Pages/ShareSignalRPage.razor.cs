using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRealTime.Client.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorRealTime.Client.Pages
{
    public partial class ShareSignalRPage : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private ShareSignalRInterop ShareSignalR => new ShareSignalRInterop(JsRuntime);
        [Parameter]
        public string AgentName { get; set; } = "adam";
        private bool isCasting = false;
        private bool notCasting => !isCasting;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var hubUrl = NavigationManager.ToAbsoluteUri("/shareScreen").ToString();
            if (firstRender)
            {
                await ShareSignalR.InitializeSignalR(hubUrl, AgentName);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task StartCasting()
        {
            await ShareSignalR.StartStreamCast(AgentName);
            isCasting = true;
        }

        private async Task StopCasting()
        {
            isCasting = false;
            await ShareSignalR.StopStreamCast();
        }

        public async ValueTask DisposeAsync()
        {
            await ShareSignalR.DisposeAsync();
        }
    }
}
