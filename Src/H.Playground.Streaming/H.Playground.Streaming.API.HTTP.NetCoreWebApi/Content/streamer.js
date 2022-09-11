(async function (window, console, fetch) {

    function Streamer(streamUrl, chunkProcessor) {

        const refreshRateInMilliseconds = 500;
        const textDecoder = new TextDecoder();

        let streamReader = null;
        let isDone = false;

        async function processStream() {
            if (!streamReader) {
                isDone = true;
                return;
            }
            if (isDone !== false)
                return;
            let streamReadResult = await streamReader.read();
            isDone = streamReadResult.done;
            await processStreamChunk(streamReadResult.value);
        }

        async function processStreamChunk(chunkValue) {
            let valueAsString = textDecoder.decode(chunkValue);
            console.debug(valueAsString);
            if (chunkProcessor)
                await chunkProcessor(valueAsString);
        }

        async function processStreamAndQueueAnother() {
            await processStream();
            if (isDone !== false)
                return;
            setTimeout(async () => {
                await processStreamAndQueueAnother();
            }, refreshRateInMilliseconds);
        }

        this.Start = async function () {

            streamReader = (await fetch(streamUrl)).body.getReader();

            await processStreamAndQueueAnother();
        }
    }

    window.Streamer = Streamer;

})(this.window, this.console, fetch);