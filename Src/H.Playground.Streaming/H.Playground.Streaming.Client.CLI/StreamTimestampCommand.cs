using H.Necessaire;

namespace H.Playground.Streaming.Client.CLI
{
    internal class StreamTimestampCommand : CliCommandBase
    {
        static readonly TimeSpan timeout = TimeSpan.FromSeconds(5);

        public override async Task<OperationResult> Run()
        {
            await Warmup();

            using (Stream contentStream = await GetHttpStream(HttpMethod.Get, $"{baseUrl}/stream/timestamp?t={(int)Math.Floor(timeout.TotalSeconds)}"))
            {
                await ProcessEndlessStream(contentStream, streamChunk =>
                {
                    if (string.IsNullOrEmpty(streamChunk))
                        return false.AsTask();

                    string[] rawValues = streamChunk.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string rawValue in rawValues)
                    {
                        if (DateTime.TryParse(rawValue.ReplaceLineEndings(string.Empty), out DateTime parseResult))
                        {
                            Log($"Server streamed timestamp: {parseResult.ToString("O")}");
                        }
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
