const zlib = require('zlib');

function parsePayload(record) {
    const payload = new Buffer(record.kinesis.data, 'base64');
    const json = (zlib.gunzipSync(payload)).toString('utf8');
    return JSON.parse(json);
}

const getRecords = (event) => event.Records.map(parsePayload);

module.exports.handler = async (event, context) => {
    // See the comments in DevNullLogShipper.PayloadExample.txt
    // to understand what is in the 'event' object.
    try {

        for (let kinesisRecord of event.Records) {

            let logMessage = parsePayload(kinesisRecord);

            if (logMessage.messageType === "DATA_MESSAGE") {
                console.log(`${logMessage.logGroup} : ${logMessage.logStream} => Contains ${logMessage.logEvents.length} messages.`);
            }
        }

    } catch (err) {
        // swallow exception so the stream can move on
        console.error(err);
    }
};