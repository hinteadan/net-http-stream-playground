using H.Necessaire.Serialization;
using H.Playground.Streaming.Core.Model;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace H.Playground.Streaming.Core
{
    public class StreamProvider
    {
        static readonly Random random = new Random();
        static readonly TimeSpan defaultDuration = TimeSpan.FromSeconds(30);
        static readonly TimeSpan maxDuration = TimeSpan.FromMinutes(5);

        readonly DataEntryProvider dataEntryProvider = new DataEntryProvider();

        public async Task StreamNumbersTo(Stream stream, TimeSpan? desiredDuration = null)
        {
            TimeSpan duration = GetActualDuration(desiredDuration);

            DateTime startedAt = DateTime.UtcNow;

            foreach (int number in new EndlessEnumerable<int>(() => random.Next(int.MinValue, int.MaxValue)))
            {
                if (DateTime.UtcNow > startedAt + duration)
                    break;

                await WriteValueToStream(stream, $"{number}{Environment.NewLine}");
            }
        }

        public async Task StreamTimestampTo(Stream stream, TimeSpan? desiredDuration = null)
        {
            TimeSpan duration = GetActualDuration(desiredDuration);

            DateTime startedAt = DateTime.UtcNow;

            foreach (DateTime dateTime in new EndlessEnumerable<DateTime>(() => DateTime.UtcNow))
            {
                if (DateTime.UtcNow > startedAt + duration)
                    break;

                await WriteValueToStream(stream, $"{dateTime.ToString("O")}{Environment.NewLine}");
            }
        }

        public async Task StreamDataEntriesTo(Stream stream, TimeSpan? desiredDuration = null)
        {
            TimeSpan duration = GetActualDuration(desiredDuration);

            DateTime startedAt = DateTime.UtcNow;

            await WriteValueToStream(stream, $"[{Environment.NewLine}");

            foreach (DataEntry dataEntry in new EndlessEnumerable<DataEntry>(dataEntryProvider.NewRandomEntry))
            {
                if (DateTime.UtcNow > startedAt + duration)
                    break;

                await WriteValueToStream(stream, $"{dataEntry.ToJsonObject()}{Environment.NewLine},{Environment.NewLine}");
            }

            await WriteValueToStream(stream, $"null{Environment.NewLine}]");
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
