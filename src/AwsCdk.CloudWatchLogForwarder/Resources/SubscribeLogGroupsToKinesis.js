const AWS = require('aws-sdk');
const cloudWatchLogs = new AWS.CloudWatchLogs();
const arn = process.env.arn;
const role_arn = process.env.role_arn;
const prefix = process.env.prefix;
const excludedLogGroups = (process.env.excluded_log_groups || '').split(';').filter(n => n);
const filterPattern = process.env.filter_pattern;

const subscribe = async (logGroupName) => {
    let options = {
        destinationArn: arn,
        logGroupName: logGroupName,
        filterName: 'ship-logs',
        filterPattern: filterPattern,
        roleArn: role_arn,
    };

    await cloudWatchLogs.putSubscriptionFilter(options).promise();
};

module.exports.handler = async (event, context) => {
    console.log(JSON.stringify(event));

    // eg. /aws/lambda/logging-demo-dev-api
    const logGroupName = event.detail.requestParameters.logGroupName;
    console.log(`log group: ${logGroupName}`);

    if (excludedLogGroups.includes(logGroupName)) {
        console.log(`ignoring the log group [${logGroupName}] because it is an excluded log group.`);
    } else if (prefix && !logGroupName.startsWith(prefix)) {
        console.log(`ignoring the log group [${logGroupName}] because it doesn't match the prefix [${prefix}]`);
    } else {
        await subscribe(logGroupName);
        console.log(`subscribed [${logGroupName}] to [${arn}]`);
    }
};
