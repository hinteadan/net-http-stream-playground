// See https://aka.ms/new-console-template for more information

using H.Necessaire.Runtime.CLI;

await
    new CliWireup()
    .WithEverything()
    .Run(askForCommandIfEmpty: true)
    ;