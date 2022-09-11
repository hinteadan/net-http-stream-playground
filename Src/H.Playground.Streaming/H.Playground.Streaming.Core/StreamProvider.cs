using H.Necessaire;
using H.Necessaire.Serialization;
using H.Playground.Streaming.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace H.Playground.Streaming.Core
{
    public class StreamProvider
    {
        static readonly TimeSpan defaultDuration = TimeSpan.FromSeconds(30);
        static readonly TimeSpan maxDuration = TimeSpan.FromMinutes(5);
        static readonly TimeSpan streamRefreshRate = TimeSpan.FromSeconds(.5);

        readonly DataEntryProvider dataEntryProvider = new DataEntryProvider();


        public async Task StreamTimestampTo(Stream stream, TimeSpan? desiredDuration = null)
        {
            await
                StreamStuff(
                    stream: stream,
                    onStart: async () => await WriteValueToStream(stream, $"{DateTime.UtcNow.ToString("O")}{Environment.NewLine}"),
                    onProgress: async () => await WriteValueToStream(stream, $"{DateTime.UtcNow.ToString("O")}{Environment.NewLine}"),
                    onEnd: null,
                    desiredDuration: desiredDuration
                );
        }

        public async Task StreamDataEntriesTo(Stream stream, TimeSpan? desiredDuration = null)
        {
            await
                StreamStuff(
                    stream: stream,
                    onStart: async () => await WriteValueToStream(stream, $"[{Environment.NewLine}"),
                    onProgress: async () =>
                    {
                        string dataEntryAsJson = dataEntryProvider.NewRandomEntry().ToJsonObject();
                        await WriteValueToStream(stream, $"{dataEntryAsJson}{Environment.NewLine},{Environment.NewLine}");
                    },
                    onEnd: async () =>
                    {
                        await WriteValueToStream(stream, $"null{Environment.NewLine}]");
                    },
                    desiredDuration: desiredDuration
                );
        }

        private async Task StreamStuff(Stream stream, Func<Task> onStart, Func<Task> onProgress, Func<Task> onEnd, TimeSpan? desiredDuration = null)
        {
            TimeSpan duration = GetActualDuration(desiredDuration);

            DateTime startedAt = DateTime.UtcNow;

            if(onStart != null)
                await onStart.TryOrFailWithGrace(onFail: null);

            while (DateTime.UtcNow <= startedAt + duration)
            {
                await Task.Delay(streamRefreshRate);

                if(onProgress != null)
                    await onProgress.TryOrFailWithGrace(onFail: null);
            }

            if(onEnd != null)
                await onEnd.TryOrFailWithGrace(onFail: null);
        }

        static async Task WriteValueToStream(Stream stream, string value)
        {
            byte[] valueAsBytes = Encoding.UTF8.GetBytes(value);
            await stream.WriteAsync(valueAsBytes, 0, valueAsBytes.Length);
            await stream.FlushAsync();
        }

        static TimeSpan GetActualDuration(TimeSpan? desiredDuration = null)
        {
            TimeSpan duration = desiredDuration ?? defaultDuration;
            duration = duration < TimeSpan.Zero ? -duration : duration;
            duration = duration > maxDuration ? maxDuration : duration;
            return duration;
        }
    }
}
