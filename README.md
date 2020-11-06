# AwsCdk.CloudWatchLogAggregator

This repository contains a C# AWS CDK custom construct (the `LogAggregator`) that makes it easy to centralize the logs of all your AWS lambdas into a single Kinesis Data Stream. The content of this Kinesis Data Stream is then forwarded to an external logging system like Loggly, Splunk, Logit.io or logz.io by invoking a custom lambda provided by the consumer. 
In other words, the repository contains a CDK based implementation of [this pattern](https://theburningmonk.com/2018/07/centralised-logging-for-aws-lambda-revised-2018/). 

For more details on the `LogAggregator` you can either:

- read this [article](https://medium.com/@beaudry.maxime/using-the-aws-cdk-in-c-to-implement-centralized-logging-for-aws-lambda-d7742aa9d6dc)
- look at the code of the [LogAggregator](src/AwsCdk.CloudWatchLogAggregator/LogAggregator.cs) and its related [LogAggregatorProps.cs](src/AwsCdk.CloudWatchLogAggregator/LogAggregatorProps.cs)
- look at the [sample code](demo/AwsCdk.CloudWatchLogAggregator.Demo) that shows how to consume the construct

The construct is available on [nuget](https://www.nuget.org/packages/AwsCdk.CloudWatchLogAggregator/).
