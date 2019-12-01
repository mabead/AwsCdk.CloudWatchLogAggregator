const zlib = require('zlib');

function parsePayload(record) {
    const payload = new Buffer(record.kinesis.data, 'base64');
    const json = (zlib.gunzipSync(payload)).toString('utf8');
    return JSON.parse(json);
}

const getRecords = (event) => event.Records.map(parsePayload);

module.exports.handler = async (event, context) => {
    try {

        console.log('before parsing: ' + JSON.stringify(event));

        const records = getRecords(event);
        console.log('after parsing: ' + JSON.stringify(records));

    } catch (err) {
        // swallow exception so the stream can move on
        console.error(err);
    }
};