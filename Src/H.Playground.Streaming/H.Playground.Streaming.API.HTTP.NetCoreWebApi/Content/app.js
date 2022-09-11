(async function (document, Streamer, JSON) {

    const timeoutInSeconds = 15;
    const jsonStringSeparator = '\r\n,\r\n';
    const stringStreamUrl = `/stream/timestamp`;
    const jsonsStreamUrl = `/stream/DataEntries`;
    const buttonStreamStrings = document.getElementById("ButtonStreamStrings");
    const buttonStreamJsons = document.getElementById("ButtonStreamJsons");
    const printCanvas = document.getElementById("PrintCanvas");
    const inputSecondsToRun = document.getElementById("InputSecondsToRun");

    async function stringChunkProcessor(chunkValue) {
        printCanvas.append(`${chunkValue}\n`);
        await Promise.resolve(true);
    }

    async function jsonChunkProcessor(chunkValue) {
        if (chunkValue.startsWith('['))
            return;

        if (chunkValue.endsWith(']'))
            return;

        let jsons = chunkValue.split(jsonStringSeparator);

        for (let i in jsons) {
            try {
                let dataEntry = JSON.parse(jsons[i]);
                console.debug(dataEntry);
                printCanvas.append(`${JSON.stringify(dataEntry, null, 2)}\n`);
            }
            catch (err) {

            }
        }

        await Promise.resolve(true);
    }

    buttonStreamStrings.addEventListener("click", async () => {
        printCanvas.innerText = '';
        await new Streamer(`${stringStreamUrl}?t=${inputSecondsToRun.value}`, stringChunkProcessor).Start();
    });

    buttonStreamJsons.addEventListener("click", async () => {
        printCanvas.innerText = '';
        await new Streamer(`${jsonsStreamUrl}?t=${inputSecondsToRun.value}`, jsonChunkProcessor).Start();
    });

})(this.document, this.window.Streamer, this.JSON);