# Sample Usage

Change your Console app's `Main` method to invoke `DockerShimRunner.Main`, as such:

```C#
using Faithlife.DockerShim;

class Program
{
  static void Main(string[] args) => DockerShimRunner.Main(new DockerShimSettings(), context =>
  {
    Console.WriteLine("Hello World!");
  });
}
```

There are three DockerShim types used in this code:

1. The code creates an `DockerShimSettings` object with the default settings. These settings control how DockerShim behaves.
1. The code invokes `DockerShimRunner.Main`, passing it the settings object and delegate representing the application logic.
1. The application logic now receives an `DockerShimContext` object with its execution context.

# Realistic Usage

DockerShim passes a `CancellationToken` to your application logic called `DockerShimContext.ExitRequested`. This token is cancelled whenever your application is requested to shut down. When `ExitRequested` is cancelled, your application logic should stop taking on new work, finish processing the current work it already has, and then return. If it does not do this, then its processing will be aborted when the application exits.

DockerShim also passes an `ILoggerFactory` to your application logic called `DockerShimContext.LoggerFactory`, which you can use to construct an `ILogger` and log to.

Finally, you should strongly consider setting `DockerShimSettings.MaximumRuntime`. Giving this property a reasonable value will ensure your application will exit after a maximum amount of time. If your application is being orchestrated (e.g., in a Kubernetes Deployment), then you can set `MaximumRuntime` to create a "phoenix service" - one that periodically exits of its own free will and is then reborn by the orchestrator.

Taking these aspects into account, a more realistic example of DockerShim usage is:

```C#
using Faithlife.DockerShim;

class MyProgram
{
  private static readonly DockerShimSettings Settings = new DockerShimSettings
  {
    MaximumRuntime = TimeSpan.FromHours(2),
  };

  static void Main(string[] args) => DockerShimRunner.Main(Settings, async context =>
  {
    // Normally loggers are created by dependency injection; this sample just creates it directly.
    var logger = context.LoggerFactory.CreateLogger<MyProgram>();

    while (true)
    {
      // Wait for the next work item to be available, and retrieve it.
      // If we are requested to exit, then cancel the wait.
      var workItem = await GetNextWorkItemAsync(context.ExitRequested);

      // Process the work item. Ignore requests to exit.
      logger.LogInformation("Processing {workItemId}", workItem.Id);
      ProcessWorkItem(workItem);
    }
  });
}
```

The actual shutdown time is randomly "fuzzed" a bit by default (see `DockerShimSettings.RandomMaximumRuntimeRelativeDelta`). The default value for this is `0.1` (i.e., +/- 10%), so the *actual* maximum runtime of the code above will vary by +/- 12 minutes (10% of 2 hours), and be a random value between 1:48 and 2:12. This is just to avoid all applications from exiting at the exact same time, even if they were all started at the same time.
