using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;

namespace ScreenCapture
{
    public partial class Screen
    {
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IJSInProcessRuntime JsInProcess { get; set; }
        private ScreenCaptureInterop ScreenCaptureInterop => new ScreenCaptureInterop(JsRuntime);
        public ElementReference VideoElementReference;
        private static Action<string> addAction;
        private List<string> captureData = new List<string>();
        protected override Task OnInitializedAsync()
        {
            addAction += AddItem;
            return base.OnInitializedAsync();
        }

        private async Task StartCapture()
        {
            await ScreenCaptureInterop.StartCapture();
            await InvokeAsync(StateHasChanged);
        }

        private void AddItem(string objString)
        {
            captureData.Add(objString);
            if (captureData.Count > 30)
            {
                captureData.RemoveAt(0);
            }
            StateHasChanged();
        }
        private async Task StopCapture()
        {
            await ScreenCaptureInterop.StopCapture();
            await InvokeAsync(StateHasChanged);
        }

        private async Task ShowCapture() => await ScreenCaptureInterop.ShowResult("VideoElementReference");

        [JSInvokable("SendScreen")]
        public static void SendScreen(object obj)
        {
            var jObject = JObject.Parse($"{obj}");
            var jsonObj = (JsonElement) obj;
            var objString = jObject.ToString();
            addAction.Invoke(objString);
            
        }
        
    }
}
