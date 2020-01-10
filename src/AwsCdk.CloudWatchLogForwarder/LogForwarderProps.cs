using Amazon.CDK;
using Amazon.CDK.AWS.Kinesis;
using Amazon.CDK.AWS.Lambda;

namespace AwsCdk.CloudWatchLogForwarder
{
    /// <summary>
    /// The properties of the LogForwarder.
    /// </summary>
    public class LogForwarderProps
    {
        /// <summary>
        /// The lambda that will be used to forward the logs from Kinesis.
        /// </summary>
        public Function LogShipper { get; }

        /// <summary>
        /// Since the logs are forwarded to Kinesis then to an external system, there 
        /// is no point in keeping the logs in CloudWatch forever. This property is used
        /// to say for how many days the logs should be kept in CloudWatch logs. By default, 
        /// logs are kept for 7 days. If you want to keep them in CloudWatch forever, set the 
        /// property to 'null'.
        /// </summary>
        public int? CloudWatchLogRetentionInDays { get; }

        /// <summary>
        /// The log forwarder will only manage the log groups that start with this prefix. 
        /// Managing a log group implies: settings its retention policy and forwarding the 
        /// logs to the log shipper lambda function.
        /// </summary>
        public string LogGroupsPrefix { get; }

        /// <summary>
        /// Only the CloudWatch logs that respect this filter pattern will be forwarded to the log shipper lambda function.
        /// </summary>
        public string CloudWatchLogsFilterPattern { get; }

        /// <summary>
        /// The properties of the Kinesis stream that will hold the CloudWatch events. When not
        /// provided, the defaults of the AWS CDK will be used.
        /// </summary>
        public IStreamProps? KinesisStreamProps { get; }

        /// <summary>
        /// The size of the batches that are sent from Kinesis to the Lambda. See https://docs.aws.amazon.com/cdk/api/latest/docs/@aws-cdk_aws-lambda-event-sources.KinesisEventSourceProps.html#batchsize for more details.
        /// </summary>
        public double? KinesisBatchSize { get; set; }

        /// <summary>
        /// The maximum amount of time to gather records in the Kinesis data stream before invoking the log shipper function. See https://docs.aws.amazon.com/cdk/api/latest/docs/@aws-cdk_aws-lambda-event-sources.KinesisEventSourceProps.html#maxbatchingwindow for more details.
        /// </summary>
        public Duration? KinesisMaxBatchingWindow { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logShipper"></param>
        /// <param name="kinesisStreamProps"></param>
        /// <param name="cloudWatchLogRetentionInDays"></param>
        /// <param name="logGroupsPrefix">The log forwarder will only manage the log groups that start with this prefix. Managing a log group implies: settings its retention policy and forwarding the logs to the log shipper lambda function. The default value is "/aws/lambda/".</param>
        /// <param name="cloudWatchLogsFilterPattern">Only the CloudWatch logs that respect this filter pattern will be forwarded to the log shipper lambda function. The default value is: -"START RequestId: " -"END RequestId: " -"REPORT RequestId: "</param>
        /// <param name="kinesisBatchSize">The size of the batches that are sent from Kinesis to the Lambda. See https://docs.aws.amazon.com/cdk/api/latest/docs/@aws-cdk_aws-lambda-event-sources.KinesisEventSourceProps.html#batchsize for more details.</param>
        /// <param name="kinesisMaxBatchingWindow">The maximum amount of time to gather records in the Kinesis data stream before invoking the log shipper function. See https://docs.aws.amazon.com/cdk/api/latest/docs/@aws-cdk_aws-lambda-event-sources.KinesisEventSourceProps.html#maxbatchingwindow for more details.</param>
        public LogForwarderProps
        (
            Function logShipper, 
            IStreamProps? kinesisStreamProps = null, 
            int? cloudWatchLogRetentionInDays = 7,
            string logGroupsPrefix = "/aws/lambda/",
            string cloudWatchLogsFilterPattern = @"-""START RequestId: "" -""END RequestId: "" -""REPORT RequestId: """,
            double? kinesisBatchSize = null,
            Duration? kinesisMaxBatchingWindow = null
        )
        {
            LogShipper = logShipper;
            KinesisStreamProps = kinesisStreamProps;
            CloudWatchLogRetentionInDays = cloudWatchLogRetentionInDays;
            LogGroupsPrefix = logGroupsPrefix;
            CloudWatchLogsFilterPattern = cloudWatchLogsFilterPattern;
            KinesisBatchSize = kinesisBatchSize;
            KinesisMaxBatchingWindow = kinesisMaxBatchingWindow ?? throw new System.ArgumentNullException(nameof(kinesisMaxBatchingWindow));
        }
    }
}
