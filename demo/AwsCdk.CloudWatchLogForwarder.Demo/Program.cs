using Amazon.CDK;
using System;

namespace AwsCdk.CloudWatchLogForwarder.Demo
{
    public class DemoStack : Stack
    {
        internal DemoStack(Construct scope, string id, IStackProps? props = null) 
            : base(scope, id, props)
        {
            var logForwarder = new LogForwarder(this, "Forwarder", 
                new LogForwarderProps(
                    cloudWatchLogRetentionInDays: 5
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
