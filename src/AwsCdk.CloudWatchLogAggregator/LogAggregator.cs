using Amazon.CDK;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using System.Collections.Generic;

namespace AwsCdk.CloudWatchLogAggregator
{
    /// <summary>
    /// The 'LogForwarder' CDK construct is responsible to *create*
    /// the following AWS resources:
    /// 
    ///  - The Kinesis Data stream that will aggregate the CloudWatch logs
    ///  - The Lambda that automatically sets the CloudWatch retention period when new lambdas are created.
    ///  - The Lambda that automatically registers the lambda CloudWatch log group to the Kinesis Data stream when new lambdas are created.
    ///  
    /// And finally, it register a 'log shipper' lambda that is invoked with the data that ends up the Kinesis Data stream.
    ///  
    /// IMPORTANT NOTE: This construct assumes that CloudTrail is enabled in your AWS environment.
    /// </summary>
    public class LogAggregator : Construct
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="id"></param>
        /// <param name="props"></param>
        public LogAggregator(Construct scope, string id, LogAggregatorProps props)
            : base(scope, id)
        {
            // The Kinesis stream that will receive all lambda logs.
            var kinesisStream = new Amazon.CDK.AWS.Kinesis.Stream(this, "Stream", props.KinesisStreamProps);

            // Forward all records from kinesis to the log shipper.
            props.LogShipper.AddEventSource(new KinesisEventSource(kinesisStream, 
                new KinesisEventSourceProps {
                    BatchSize = props.KinesisBatchSize,
                    MaxBatchingWindow = props.KinesisMaxBatchingWindow,
                    StartingPosition = StartingPosition.TRIM_HORIZON,
                }));

            // Create a role (containing a policy) that can be assumed by CloudWatch logs to put records in Kinesis. 
            var statement = new PolicyStatement();
            statement.AddActions(new[] { "kinesis:PutRecords", "kinesis:PutRecord" });
            statement.AddResources(new[] { kinesisStream.StreamArn });

            var cloudWatchLogsToKinesisRole = new Role(this, "CloudWatchLogsToKinesis", new RoleProps
            {
                AssumedBy = new ServicePrincipal("logs.amazonaws.com")
            });
            cloudWatchLogsToKinesisRole.AttachInlinePolicy(new Policy(this, "CanPutRecordsInKinesis", new PolicyProps
            {
               Statements = new[] { statement }    
            }));

            // This CloudWatch rule will trigger whenever a new LogGroup is created. This assumes 
            // that CloudTrail is enabled.
            var createLogGroupEventRule = new Rule(this, "CreateLogGroupEvent", new RuleProps {
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

            if (props.CloudWatchLogRetentionInDays.HasValue)
            {
                var setLogGroupExpirationLambda = new Function(this, "SetLogGroupExpiration", new FunctionProps
                {
                    Runtime = Runtime.NODEJS_10_X,
                    Handler = "index.handler",
                    Description = $"Sets the log retention policy to {props.CloudWatchLogRetentionInDays} days when a log group is created.",
                    MemorySize = 128,
                    Environment = new Dictionary<string, string>
                    {
                        { "retention_days",     props.CloudWatchLogRetentionInDays.Value.ToString() },
                        { "prefix",             props.LogGroupsPrefix },
                    },
                    Code = Code.FromInline(EmbeddedResourceReader.Read("Resources.SetExpiry.js"))
                });

                createLogGroupEventRule.AddTarget(new LambdaFunction(setLogGroupExpirationLambda));
            }

            var excludedLogGroups = props.LogShipper == null ? "" : $"/aws/lambda/{props.LogShipper.FunctionName}";

            // This function will be invoked whenever a CloudWatch log group is created. It will
            // subscribe the log group to our Kinesis Data Stream so that all logs end up in the 
            // DataStream.
            var subscribeLogGroupsToKinesisLambda = new Function(this, "SubscribeLogGroupsToKinesis", new FunctionProps
            {
                Runtime = Runtime.NODEJS_10_X,
                Handler = "index.handler",
                Description = "Subscribe logs to the Kinesis stream",
                MemorySize = 128,
                Environment = new Dictionary<string, string>
                {
                    { "arn",                    kinesisStream.StreamArn },
                    { "role_arn",               cloudWatchLogsToKinesisRole.RoleArn },
                    { "prefix",                 props.LogGroupsPrefix },
                    { "excluded_log_groups",    excludedLogGroups },
                    { "filter_pattern",         props.CloudWatchLogsFilterPattern.ToString() },
                },
                Code = Code.FromInline(EmbeddedResourceReader.Read("Resources.SubscribeLogGroupsToKinesis.js"))
            });

            subscribeLogGroupsToKinesisLambda.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new [] { "logs:PutSubscriptionFilter" },
                Resources = new[] { "*" },
            }));

            subscribeLogGroupsToKinesisLambda.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "iam:PassRole" },
                Resources = new[] { "*" },
            }));

            createLogGroupEventRule.AddTarget(new LambdaFunction(subscribeLogGroupsToKinesisLambda));
        }
    }
}
