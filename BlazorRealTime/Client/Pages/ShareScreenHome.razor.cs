using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRealTime.Client.Interop;
using BlazorRealTime.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace BlazorRealTime.Client.Pages
{
    public partial class ShareScreenHome
    {
        private string agentName;
        //private string message;
        private bool isShareReady;
        [Inject]
        private IJSRuntime JsRuntime { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private ShareSignalRInterop ShareSignalR => new ShareSignalRInterop(JsRuntime);

        private List<string> agents = new List<string>();
        private static Action<ChatMessage> receiveChatAction;
        //private HubConnection connection;
        private List<ChatMessage> messages = new List<ChatMessage>();

        protected override async Task OnInitializedAsync()
        {
            receiveChatAction = OnMessageReceived;
            //connection = new HubConnectionBuilder()
            //    .WithUrl(NavigationManager.ToAbsoluteUri("/shareScreen"))
            //    .Build();
            //connection.On<ChatMessage>("ReceiveMessage", OnMessageReceived);

            //await connection.StartAsync();
            await base.OnInitializedAsync();
        }
        private void StartShare()
        {
            isShareReady = !isShareReady;
            StateHasChanged();
        }
        private void OnMessageReceived(ChatMessage chatMessage)
        {
            messages.Add(chatMessage);
            if (messages.Count > 20)
                messages.RemoveAt(0);
            StateHasChanged();
        }
        [JSInvokable("ReceiveChat")]
        public static void ReceiveJsChat(ChatMessage chatMessage)
        {
            Console.WriteLine($"Chat from JS: {chatMessage.Sender} said {chatMessage.Message}");
            receiveChatAction.Invoke(chatMessage);
        }
        private async Task SendChat(string message = "Empty-Chat")
        {
            await ShareSignalR.SendChatMessage(agentName, new ChatMessage(agentName, message));
            //await connection.SendAsync("SendChat", agentName, new ChatMessage(agentName, message));
        }
        private void OnEnter(KeyboardEventArgs args)
        {
            if (args.Key.ToUpper() == "ENTER")
            {
                StartShare();
            }
        }
    }
}
