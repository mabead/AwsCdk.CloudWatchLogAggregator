using Amazon.CDK;
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
                        { "LOG_GROUP_RETENTION", props.CloudWatchLogRetentionInDays.ToString() }
                    },
                    Code = Code.FromInline(ReadEmbeddedResource("Resources."))
                });
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
