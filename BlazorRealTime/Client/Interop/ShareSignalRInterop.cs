using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRealTime.Shared;
using Microsoft.JSInterop;

namespace BlazorRealTime.Client.Interop
{
    public class ShareSignalRInterop
    {
        private readonly Lazy<Task<IJSObjectReference>> screenModule;

        public ShareSignalRInterop(IJSRuntime jsRuntime)
        {
            screenModule = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/signalRInterop.js").AsTask());
        }

        public async ValueTask InitializeSignalR(string hostUrl, string userName)
        {
            var module = await screenModule.Value;
            await module.InvokeVoidAsync("initializeSignalR", hostUrl, userName);
        }

        public async ValueTask StartStreamCast(string userName)
        {
            var module = await screenModule.Value;
            await module.InvokeVoidAsync("startCast",userName);
        }
        public async ValueTask StopStreamCast()
        {
            var module = await screenModule.Value;
            await module.InvokeVoidAsync("stopStreamCast");
        }

        public async ValueTask SendChatMessage(string name, ChatMessage chatMessage)
        {
            var module = await screenModule.Value;
            await module.InvokeVoidAsync("sendChat", name, chatMessage);
        }
        public async ValueTask DisposeAsync()
        {
            if (screenModule.IsValueCreated)
            {
                var module = await screenModule.Value;
                await module.DisposeAsync();
            }
        }
    }
}
