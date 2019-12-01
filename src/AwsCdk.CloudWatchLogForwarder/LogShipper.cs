using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;

namespace AwsCdk.CloudWatchLogForwarder
{
    public static class LogShipper
    {
        public static Function CreateDevNullLogShipper(Construct scope)
        {
            return new Function(scope, "DevNullLogShipper", new FunctionProps
            {
                // TODO MAX: function name?
                FunctionName = "DevNullLogShipper",
                Runtime = Runtime.NODEJS_10_X,
                Handler = "index.handler",
                Description = "Forwards kinesis records to nowhere.",
                MemorySize = 128,
                Code = Code.FromInline(EmbeddedResourceReader.Read("Resources.DevNullLogShipper.js"))
            });
        }
    }
}
