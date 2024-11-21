## AndroThink.WebJob.Core [![Nuget](https://img.shields.io/nuget/v/AndroThink.WebJob.Core)](https://www.nuget.org/packages/AndroThink.WebJob.Core)
Library that help in configuring new dot net core web job project

![](https://raw.githubusercontent.com/AndroThink/WebJobCore/main/AndroThink.WebJob.Core/Images/andro_think.png)

## How to use 

 ### In Your Control
```c#
var job = CoreWebJob.Create("appsettings.json", true)
    .ConfigureWebJobs(options =>
    {

    }).ConfigureLogging((context, logging) =>
    {
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IMailSender,MailSender>();

        services.AddHostedService<TestService>(); // this inherites from BaseBackgroundService
    })
    .WithFileLogger("Logs")
    .UseConsoleLifetime();

await job.RunAsync((sender, args) =>
{
    Exception ex = (Exception)args.ExceptionObject;

    Console.WriteLine("Webjob exception ==> " + ex.ToString());
});

```
