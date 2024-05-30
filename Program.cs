using Hangfire;
using HangfireTutorial;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(x =>
    x.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage("Server=localhost;Database=Hangfire;User Id=sa;Password=ChangeMe1;TrustServerCertificate=True;")
    );

builder.Services.AddTransient<ExampleJob>();

builder.Services.AddHangfireServer(x => x.SchedulePollingInterval = TimeSpan.FromSeconds(1));

var app = builder.Build();

app.UseHangfireDashboard();

app.MapGet("/job", (IBackgroundJobClient jobClient, IRecurringJobManager jobManager) =>
{
    //jobClient.Enqueue(() => Console.WriteLine("Hello from BG"));
    
    jobClient.Schedule(() => Console.WriteLine("Hello from BG 2"), TimeSpan.FromSeconds(5));
    //jobManager.AddOrUpdate("every5second", () => Console.WriteLine("Hello from BG 3"), "*/5 * * * *");
    
    jobClient.Schedule<ExampleJob>(x => x.DoSomething(), TimeSpan.FromSeconds(5));
    return Results.Ok("Hello job!");
});



app.Run();
