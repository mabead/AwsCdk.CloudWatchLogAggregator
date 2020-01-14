using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;

namespace AwsCdk.CloudWatchLogForwarder
{
    /// <summary>
    /// Contains a sample implementation of 'LogShipper' lambda.
    /// </summary>
    public static class LogShipper
    {
        /// <summary>
        /// Creates a log shipper that does nothing. It's like sending the logs to /dev/null.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static Function CreateDevNullLogShipper(Construct scope)
        {
            return new Function(scope, "DevNullLogShipper", new FunctionProps
            {
                Runtime = Runtime.NODEJS_10_X,
                Handler = "index.handler",
                Description = "Forwards kinesis records to nowhere. It simply logs that it received something.",
                MemorySize = 128,
                Code = Code.FromInline(EmbeddedResourceReader.Read("Resources.DevNullLogShipper.js"))
            });
        }
    }
}
