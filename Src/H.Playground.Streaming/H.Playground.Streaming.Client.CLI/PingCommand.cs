using H.Necessaire;
using H.Necessaire.Runtime.CLI;

namespace H.Playground.Streaming.Client.CLI
{
    internal class PingCommand : CliCommandBase
    {
        public override async Task<OperationResult> Run()
        {
            await Warmup();

            string pingResponsePayload = await httpClient.GetStringAsync($"{baseUrl}");

            Log($"Ping HTTP Response: {pingResponsePayload}");

            Log("Press <ENTER> to exit");

            await WaitForUserInput();

            return
                pingResponsePayload
                .And(x => CLIPrinter.PrintLog(pingResponsePayload))
                .ToWinResult()
                ;
        }
    }
}
