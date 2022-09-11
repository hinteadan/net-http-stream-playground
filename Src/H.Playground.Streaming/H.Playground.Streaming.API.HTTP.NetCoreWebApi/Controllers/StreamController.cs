using H.Necessaire;
using H.Playground.Streaming.Core;
using Microsoft.AspNetCore.Mvc;

namespace H.Playground.Streaming.API.HTTP.NetCoreWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StreamController : ControllerBase
    {
        StreamProvider streamProvider;
        public StreamController()
        {
            streamProvider = new StreamProvider();
        }

        [HttpGet]
        [Route(nameof(Timestamp))]
        public async Task Timestamp([FromQuery] string? t)
        {
            Response.ContentType = "text/plain; charset=utf-8";
            double? desiredDurationInSeconds = t?.ParseToDoubleOrFallbackTo(null);
            await streamProvider.StreamTimestampTo(Response.Body, desiredDuration: desiredDurationInSeconds == null ? null : TimeSpan.FromSeconds(desiredDurationInSeconds.Value));
        }

        [HttpGet]
        [Route(nameof(DataEntries))]
        public async Task DataEntries([FromQuery] string? t)
        {
            Response.ContentType = "text/plain; charset=utf-8; x-subtype=json";
            double? desiredDurationInSeconds = t?.ParseToDoubleOrFallbackTo(null);
            await streamProvider.StreamDataEntriesTo(Response.Body, desiredDuration: desiredDurationInSeconds == null ? null : TimeSpan.FromSeconds(desiredDurationInSeconds.Value));
        }
    }
}
