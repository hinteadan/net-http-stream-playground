using H.Necessaire;
using H.Necessaire.Serialization;
using H.Playground.Streaming.Core.Model;

namespace H.Playground.Streaming.Client.CLI
{
    internal class StreamDataEntryCommand : CliCommandBase
    {
        static readonly TimeSpan timeout = TimeSpan.FromSeconds(5);
        static readonly string separator = $"{Environment.NewLine},{Environment.NewLine}";

        public override async Task<OperationResult> Run()
        {
            await Warmup();

            using (Stream contentStream = await GetHttpStream(HttpMethod.Get, $"{baseUrl}/stream/DataEntries?t={(int)Math.Floor(timeout.TotalSeconds)}"))
            {
                await ProcessEndlessStream(contentStream, streamChunk =>
                {
                    if (string.IsNullOrEmpty(streamChunk))
                        return false.AsTask();

                    if (streamChunk.StartsWith("["))
                        streamChunk = streamChunk.Substring(1);

                    if (streamChunk.EndsWith("]"))
                        streamChunk = streamChunk.Substring(0, streamChunk.Length - 1);

                    string[] rawValues = streamChunk.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string rawValue in rawValues)
                    {
                        DataEntry parsedEntry = rawValue.JsonToObject<DataEntry>();

                        if (parsedEntry is null)
                            continue;

                        Log($"Server Data Entry: {parsedEntry}");
                    }

                    return true.AsTask();
                });
            }

            Log("Press <ENTER> to exit");

            await WaitForUserInput();

            return
                true
                .ToWinResult()
                ;
        }
    }
}
