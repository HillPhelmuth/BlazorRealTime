using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ScreenCapture
{
    public class ScreenCaptureInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> screenModule;
       // private readonly Lazy<Task<IJSInProcessObjectReference>> screenSync;

        public ScreenCaptureInterop(IJSRuntime jsRuntime)
        {
            screenModule = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/ScreenCapture/screenCaptureInterop.js").AsTask());
            //screenSync = new(() => jsInProcess.InvokeAsync<IJSInProcessObjectReference>("import", "./_content/ScreenCapture/screenCaptureInterop.js").AsTask());
        }

        public async ValueTask StartCapture()
        {
            var module = await screenModule.Value;
            await module.InvokeVoidAsync("startCapture");
        }


        public async ValueTask ShowResult(string element)
        {
            var module = await screenModule.Value;
            Console.WriteLine($"ShowResult received data");
            module.InvokeVoidAsync("setCapture", element);
        }

        public async ValueTask StopCapture()
        {
            var module = await screenModule.Value;
            await module.InvokeVoidAsync("stopCapture");
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
