using System;
using System.IO;

namespace AwsCdk.CloudWatchLogAggregator
{
    internal static class EmbeddedResourceReader
    {
        public static string Read(string resourcePath)
        {
            var assembly = typeof(EmbeddedResourceReader).Assembly;
            var assemblyName = assembly.GetName();
            var fullPath = $"{assemblyName.Name}.{resourcePath}";

            using (var stream = assembly.GetManifestResourceStream(fullPath))
            {
                if (stream == null)
                {
                    throw new NotSupportedException($"Embedded resource {resourcePath} was not found.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
