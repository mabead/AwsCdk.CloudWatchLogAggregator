using Amazon.CDK;

namespace AwsCdk.CloudWatchLogForwarder.Demo
{
    public class DemoStack : Stack
    {
        internal DemoStack(Construct scope, string id, IStackProps? props = null) 
            : base(scope, id, props)
        {
            new LogForwarder(this, "Forwarder", 
                new LogForwarderProps(
                    // For the sake of the demo, we connect a 'passthrough' 
                    // (similar to /dev/null) lambda to Kinesis. We will 
                    // therefore see that the lambda is invoked but does 
                    // nothing with the logs. If you look at the code of 
                    // the 'CreateDevNullLogShipper', you will see that it 
                    // is quite easy to implement a log shipper. In a real 
                    // use you would create a custom log shipper that forwards
                    // to something like loggly, splunk, logz.io, etc.
                    logShipper: LogShipper.CreateDevNullLogShipper(this),

                    // In this demo, only the lambda that start with the 
                    // name 'Demo' will have their log forwarded to the 
                    // Kinesis stream and then to the 'DevNull' log shipper.
                    logGroupsPrefix: "/aws/lambda/Demo",

                    // Since the logs have been forwarded to an external 
                    // system like loggly, splunk or logz.io, there is no 
                    // point in keeping the logs for a long time in CloudWatch.
                    // So all the lambdas that fit the 'logGroupsPrefix' will 
                    // have their CloudWatch logs retention automatically 
                    // set to 5 days instead of the default 'infinite' 
                    // retention. The default retention can quickly become 
                    // costly. 
                    cloudWatchLogRetentionInDays: 5,

                    // To have faster feedback in this demo, we use a very
                    // small value for the Kinesis batch size. So, as soon
                    // as there is 1 message available in the Kinesis stream,
                    // the log shipper will be invoked.
                    kinesisBatchSize: 1

                    // NOTE: 
                    //
                    // There are a few more parameters that are available.
                    // See the code of 'LogForwarderProps' for more details.
                ));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new App();
            new DemoStack(app, "CloudWatchLogForwarderDemo", new StackProps
            {
                Env = new Amazon.CDK.Environment { Region = "us-east-1" },
            });

            app.Synth();
        }
    }
}
