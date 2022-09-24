(async function (document, Streamer, JSON) {

    const defaultSeparator = '\r\n';
    const jsonStringSeparator = '\r\n,\r\n';
    const stringStreamUrl = `/stream/timestamp`;
    const jsonsStreamUrl = `/stream/DataEntries`;
    const numbersStreamUrl = `/stream/Numbers`;
    const buttonStreamStrings = document.getElementById("ButtonStreamStrings");
    const buttonStreamJsons = document.getElementById("ButtonStreamJsons");
    const buttonStreamNumbers = document.getElementById("ButtonStreamNumbers");
    const printCanvas = document.getElementById("PrintCanvas");
    const inputSecondsToRun = document.getElementById("InputSecondsToRun");

    async function stringChunkProcessor(chunkValue) {
        let values = chunkValue.split(defaultSeparator);
        for (let i in values) {
            let value = values[i];
            if (!value) continue;
            console.debug(value);
            printCanvas.append(`${value}\n`);
        }
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

    async function numberChunkProcessor(chunkValue) {
        let values = chunkValue.split(defaultSeparator);
        for (let i in values) {
            let value = values[i];
            if (!value) continue;
            console.debug(value);
            printCanvas.append(`${value}\n`);
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

    buttonStreamNumbers.addEventListener("click", async () => {
        printCanvas.innerText = '';
        await new Streamer(`${numbersStreamUrl}?t=${inputSecondsToRun.value}`, numberChunkProcessor).Start();
    });

})(this.document, this.window.Streamer, this.JSON);