using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BlazorRealTime.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using ScreenCapture;

namespace BlazorRealTime.Client.Pages
{
    public partial class ScreenShare : IAsyncDisposable
    {
        [Inject]
        private ScreenCaptureInterop ScreenCaptureInterop { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private HttpClient HttpClient { get; set; }
        private HubConnection hubConnection;
        private List<string> messages = new List<string>();
        public ElementReference VideoElementReference;
        private static Action action;
        private string userInput;
        private string messageInput;
        private string imgUrl;
        
        private static Queue<string> dataQueue = new Queue<string>();
       
        protected override async Task OnInitializedAsync()
        {
            action = HandleAction;
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/shareScreen"))
                .AddJsonProtocol(o => o.PayloadSerializerOptions.DefaultBufferSize = 2024000)
                .Build();

            hubConnection.On<string>("Joined", (user) =>
            {
                messages.Add(user);
                StateHasChanged();
            });

            hubConnection.On<byte[]>("DrawFrame", async element =>
            {

                var imgData = Convert.ToBase64String(element);
                Console.WriteLine($"received data from DrawFrame: {imgData.Substring(0,10)}...");
                await ScreenCaptureInterop.ShowResult(imgData);
                imgUrl = imgData;
                StateHasChanged();
            });
            hubConnection.On<ScreenDataItem>("HandleStreamData", async data =>
            {
                Console.WriteLine($"Stream handled: {data.ToString().Substring(0, 10)}");
                imgUrl = $"data:image/png;base64,{data}";  
                _ = ScreenCaptureInterop.ShowResult(imgUrl);
                await InvokeAsync(StateHasChanged);
            });
            await hubConnection.StartAsync();
            await hubConnection.SendAsync("Joined", "Adam");
            
            await base.OnInitializedAsync();
        }

        private async void HandleAction()
        {
            Console.WriteLine("Handled Action");
           
            await SendData();
        }

        private async Task SendData(string dataUrl = "")
        {
            var random = new Random();
            var id = random.Next(1, 5000);
            var value = dataQueue.TryDequeue(out var result) ? result : "";
            await HttpClient.PostAsJsonAsync("api/screenData/send", new ScreenDataItem {Value = value, Id = id});
            //var data = Convert.FromBase64String(dataUrl);
            //await hubConnection.SendAsync("SendElement", $@"{dataUrl}");
        }
        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;
        private async Task StartCapture()
        {
            await ScreenCaptureInterop.StartCapture();
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task StopCapture()
        {
            await ScreenCaptureInterop.StopCapture();
            await InvokeAsync(StateHasChanged);
        }

       
        [JSInvokable("SendScreen")]
        public static void Send(object objTemp)
        {
            
            Console.WriteLine($"Serialized message from JS: \r\n {JsonSerializer.Serialize(objTemp.ToString())}");
            dataQueue.Enqueue(objTemp.ToString());
            //dataList.Add(objTemp.ToString());
            action.Invoke();
        }
        public async ValueTask DisposeAsync()
        {
            await hubConnection.DisposeAsync();
        }


    }
}
