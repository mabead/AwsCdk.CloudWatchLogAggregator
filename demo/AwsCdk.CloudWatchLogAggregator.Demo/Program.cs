using Amazon.CDK;

namespace AwsCdk.CloudWatchLogAggregator.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App();
            new DemoStack(app, "CloudWatchLogAggregatorDemo", new StackProps
            {
                Env = new Amazon.CDK.Environment { Region = "us-east-1" },
            });

            app.Synth();
        }
    }
}
