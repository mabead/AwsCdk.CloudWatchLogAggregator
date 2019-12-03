using Amazon.CDK.AWS.Kinesis;
using Amazon.CDK.AWS.Lambda;

namespace AwsCdk.CloudWatchLogForwarder
{
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
        /// Only the CloudWatch logs that respect this filter pattern will be forwarded to the log shipper lambda function.
        /// </summary>
        public string CloudWatchLogsFilterPattern { get; }

        /// <summary>
        /// The properties of the Kinesis stream that will hold the CloudWatch events. When not
        /// provided, the defaults of the AWS CDK will be used.
        /// </summary>
        public IStreamProps? KinesisStreamProps { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logShipper"></param>
        /// <param name="kinesisStreamProps"></param>
        /// <param name="cloudWatchLogRetentionInDays"></param>
        /// <param name="cloudWatchLogsFilterPattern">Only the CloudWatch logs that respect this filter pattern will be forwarded to the log shipper lambda function. The default value is: -"START RequestId: " -"END RequestId: " -"REPORT RequestId: "</param>
        public LogForwarderProps
        (
            Function logShipper, 
            IStreamProps? kinesisStreamProps = null, 
            int? cloudWatchLogRetentionInDays = 7,
            string cloudWatchLogsFilterPattern = @"-""START RequestId: "" -""END RequestId: "" -""REPORT RequestId: """
        )
        {
            LogShipper = logShipper;
            KinesisStreamProps = kinesisStreamProps;
            CloudWatchLogRetentionInDays = cloudWatchLogRetentionInDays;
            CloudWatchLogsFilterPattern = cloudWatchLogsFilterPattern;
        }
    }
}
