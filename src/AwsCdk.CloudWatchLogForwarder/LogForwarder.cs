using Amazon.CDK;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using System.Collections.Generic;
using System.IO;

namespace AwsCdk.CloudWatchLogForwarder
{
    public class LogForwarder : Construct
    {
        public Function SetLogGroupExpirationLambda { get; } = null;

        public LogForwarder(Construct scope, string id, LogForwarderProps props = null)
            : base(scope, id)
        {
            props ??= new LogForwarderProps();

            // The Kinesis stream that will receive all lambda logs.
            var stream = new Amazon.CDK.AWS.Kinesis.Stream(this, "Stream", props?.KinesisStreamProps);

            // This CloudWatch rule will trigger whenever a new LogGroup is created. This assumes 
            // that CloudTrail is enabled.
            var createLogGroupEventRule = new Rule(this, "CreateLogGroupEvent", new RuleProps {
                // TODO MAX: name event?
                RuleName = "LogGroupCreated",
                Description = "Fires whenever CloudTrail detects that a log group is created",
                EventPattern = new EventPattern {
                    Source = new[] { "aws.logs" },
                    DetailType = new[] { "AWS API Call via CloudTrail" },
                    Detail = new Dictionary<string, object>
                    {
                        { "eventSource", new string[] { "logs.amazonaws.com" } },
                        { "eventName", new string[] { "CreateLogGroup" } },
                    }
                }});

            if (props?.CloudWatchLogRetentionInDays != null)
            {
                SetLogGroupExpirationLambda = new Function(this, "SetLogGroupExpiration", new FunctionProps
                {
                    // TODO MAX: function name
                    FunctionName = "SetLogGroupExpiration",
                    Runtime = Runtime.NODEJS_10_X,
                    Handler = "index.handler",
                    Description = $"Sets the log retention policy to {props.CloudWatchLogRetentionInDays} days when a log group is created.",
                    MemorySize = 256,
                    Environment = new Dictionary<string, string>
                    {
                        { "retention_days", props.CloudWatchLogRetentionInDays.ToString() }
                    },
                    Code = Code.FromInline(ReadEmbeddedResource("Resources.SetExpiry.js"))
                });

                createLogGroupEventRule.AddTarget(new LambdaFunction(SetLogGroupExpirationLambda));
            }
        }

        private string ReadEmbeddedResource(string resourcePath)
        {
            var assembly = GetType().Assembly;
            var assemblyName = assembly.GetName();
            var fullPath = $"{assemblyName.Name}.{resourcePath}";

            using (var stream = assembly.GetManifestResourceStream(fullPath))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
