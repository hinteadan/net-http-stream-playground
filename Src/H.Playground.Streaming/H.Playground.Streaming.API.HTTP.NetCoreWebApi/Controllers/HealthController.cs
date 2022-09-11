using H.Necessaire;
using Microsoft.AspNetCore.Mvc;

namespace H.Playground.Streaming.API.HTTP.NetCoreWebApi.Controllers
{
    [ApiController]
    [Route("")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public Task<string> Pong()
        {
            return $"Alive @ {DateTime.UtcNow} UTC".AsTask();
        }
    }
}
