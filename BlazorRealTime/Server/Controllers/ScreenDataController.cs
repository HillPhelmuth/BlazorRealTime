using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorRealTime.Server.Hubs;
using BlazorRealTime.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorRealTime.Server.Controllers
{
    [Route("api/screenData")]
    [ApiController]
    public class ScreenDataController : ControllerBase
    {
        private readonly IHubContext<ShareScreenHub> _sharHubContext;

        public ScreenDataController(IHubContext<ShareScreenHub> sharHubContext)
        {
            _sharHubContext = sharHubContext;
        }

        [HttpPost("send")]
        public async Task SendScreenData([FromBody] ScreenDataItem item)
        {
            Console.WriteLine($"Data {item?.Value?.Substring(0, 10)}... made it to server controller");
            await _sharHubContext.Clients.All.SendAsync("HandleStreamData", item);
        }
    }
}
