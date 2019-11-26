using Amazon.CDK.AWS.Kinesis;

namespace AwsCdk.CloudWatchLogForwarder
{
    public class LogForwarderProps
    {
        /// <summary>
        /// Since the logs are forwarded to Kinesis then to an external system, there 
        /// is no point in keeping the logs in CloudWatch forever. This property is used
        /// to say for how many days the logs should be kept in CloudWatch logs. By default, 
        /// logs are kept for 7 days. If you want to keep them in CloudWatch forever, set the 
        /// property to 'null'.
        /// </summary>
        public int? CloudWatchLogRetentionInDays { get; }

        /// <summary>
        /// The properties of the Kinesis stream that will hold the CloudWatch events. When not
        /// provided, the defaults of the AWS CDK will be used.
        /// </summary>
        public IStreamProps KinesisStreamProps { get; } 

        public LogForwarderProps(IStreamProps kinesisStreamProps = null, int? cloudWatchLogRetentionInDays = 7)
        {
            KinesisStreamProps = kinesisStreamProps;
            CloudWatchLogRetentionInDays = cloudWatchLogRetentionInDays;
        }
    }
}
