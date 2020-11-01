using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRealTime.Client.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorRealTime.Client.Pages
{
    public partial class ShareSignalRPage
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private ShareSignalRInterop ShareSignalR => new ShareSignalRInterop(JsRuntime);
        private string agentName = "adam";
        private bool isCasting = false;
        private bool notCasting => !isCasting;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var hubUrl = NavigationManager.ToAbsoluteUri("/shareScreen").ToString();
            if (firstRender)
            {
                await ShareSignalR.InitializeSignalR(hubUrl, agentName);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task StartCasting()
        {
            await ShareSignalR.StartStreamCast(agentName);
            isCasting = true;
        }

        private async Task StopCasting()
        {
            isCasting = false;
            await ShareSignalR.StopStreamCast();
        }
    }
}
