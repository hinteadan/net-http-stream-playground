using H.Necessaire;
using H.Necessaire.Runtime.CLI.Commands;
using System.Text;

namespace H.Playground.Streaming.Client.CLI
{
    internal abstract class CliCommandBase : CommandBase
    {
        protected const string baseUrl = "http://localhost:5261";
        protected static readonly HttpClient httpClient = new HttpClient();
        static readonly TimeSpan processingRefreshRate = TimeSpan.FromSeconds(.5);
        static readonly TimeSpan warmupDuration = TimeSpan.FromSeconds(3);
        static readonly string defaultEndMarker = Environment.NewLine;
        const uint blankStreamReadCountTolerance = 3;
        const uint streamBufferSize = 512;

        protected async Task Warmup()
        {
            Log("Warming up...");
            Log($"0% warmed up. {Math.Round(warmupDuration.TotalSeconds, 0)} second(s) remaining.");

            for (double second = warmupDuration.TotalSeconds; second > 0; second -= processingRefreshRate.TotalSeconds)
            {
                await Task.Delay(processingRefreshRate);
                Log($"{Math.Round((warmupDuration.TotalSeconds - second) / warmupDuration.TotalSeconds * 100, 0)}% warmed up. {Math.Round(second, 1)} second(s) remaining.");
            }

            Log("100% All warmed up. 0 second(s) remaining.");
        }

        protected Task<string?> WaitForUserInput()
        {
            string? userInput = Console.ReadLine();

            return userInput.AsTask();
        }

        protected async Task ProcessEndlessStream(Stream stream, Func<string, Task> chunkProcessor, TimeSpan? timeout = null)
        {
            DateTime startedAt = DateTime.Now;
            uint blankReadCount = 0;
            while ((blankReadCount <= blankStreamReadCountTolerance) && (timeout is null ? true : DateTime.Now <= startedAt + timeout.Value))
            {
                string chunkValue = await ReadChunckFromStream(stream);

                if (chunkValue.Length == 0)
                    blankReadCount++;

                await chunkProcessor(chunkValue);

                await Task.Delay(processingRefreshRate);
            };
        }

        protected async Task<Stream> GetHttpStream(HttpMethod httpMethod, string url)
        {
            using (HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, url))
            {
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

                httpResponse.EnsureSuccessStatusCode();

                return await httpResponse.Content.ReadAsStreamAsync();
            }
        }

        private static async Task<string> ReadChunckFromStream(Stream stream, params string?[] endMarkers)
        {
            StringBuilder resultBuilder = new StringBuilder();

            byte[] buffer = new byte[streamBufferSize];
            string? readString = null;

            int readLength = -1;

            do
            {
                readLength = await stream.ReadAsync(buffer, 0, buffer.Length);

                readString = Encoding.UTF8.GetString(buffer).TrimEnd('\0');

                resultBuilder.Append(readString);

                Array.Clear(buffer, 0, readLength);
            }
            while (readLength > 0 && (endMarkers?.Any() == true ? endMarkers : defaultEndMarker.AsArray()).All(endMarker => readString?.EndsWith(endMarker ?? defaultEndMarker) != true));

            return resultBuilder.ToString();
        }
    }
}
