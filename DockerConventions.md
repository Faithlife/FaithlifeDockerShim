# Doker Conventions

## Exit Codes

A DockerShim process will return one of the following exit codes:

* `0` - If the application logic returns without exception.
* `64` - If the application logic directly threw an unhandled exception. Unhandled exceptions are logged before the process exits.
* `65` - If the application logic indirectly threw an unhandled exception (e.g., from a thread pool thread). Unhandled exceptions are logged before the process exits.
* `66` - If the application logic was requested to shutdown, but did not do so within the exit timeout (see `DockerShimSettings.ExitTimeout`).
* (other) - If the application logic returns an `int`, then that value is used as the process exit code.

Exit codes are returned by DockerShim even if you use `static void Main` as your entrypoint.

## Signals

DockerShim responds to `Ctrl-C` (if your container is run interactively) as well as `docker stop` on all platforms and container types. Note that for `docker stop` to work with Windows containers, they must have a base image *and* host running Windows Version `1709` or higher.

### Signal Specifics

DockerShim listens to various signals based on OS:
* Windows: `CTRL_C_EVENT`, `CTRL_CLOSE_EVENT`, and `CTRL_SHUTDOWN_EVENT`
* Other: `SIGINT` and `SIGTERM`

These are all treated the same: as a graceful stop request. When one of these signals is received, the `DockerShimContext.ExitRequested` cancellation token is cancelled. When this token is cancelled, your code should stop taking on new work. It should complete the work it already has and then exit.

When a signal comes in, DockerShim will start a kill timer (see `DockerShimSettings.ExitTimeout`). If the application code has not returned within that timeout, DockerShim will exit the process with exit code `66`.

## Logs

Docker expects logs to be written to stdout (or stderr), with *one line per log message*.

DockerShim has a core logging factory, exposed at `DockerShimContext.LoggingFactory`. All of DockerShim's logs go through this factory (using the `"DockerShim"` category/logger name), and this same factory can be used to create application logs.

By default, all log messages sent to `DockerShimContext.LoggingFactory` are formatted on a single line using backslash-escaping. These lines are then written to stdout.

### Redirecting DockerShim Logs

Docker applications that do their own application logging directly will want to redirect DockerShim's logging. This is done by setting `DockerShimSettings.LoggingFactory` before calling into `DockerShimRunner.Main`. DockerShim will then use the provided `ILoggerFactory` instead of its own factory and provider.

```C#
static void Main(string[] args)
{
  var myLoggerFactory = new LoggerFactory();
  myLoggerFactory.AddMyOwnProvider(); // log4net, seq, gelf, whatever...

  var settings = new DockerShimSettings
  {
    MaximumRuntime = TimeSpan.FromHours(2),
    LoggerFactory = myLoggerFactory,
  };
  DockerShimRunner.Main(settings, context =>
  {
    var loggerFactory = context.LoggerFactory; // Same instance as `myLoggerFactory` that we passed into the settings.
  });
}
```

This way you can send DockerShim's own logs to your customized logging provider instead of as stdout to Docker.
