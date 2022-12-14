using NewPortfolioMicroservice.Definitions.Base;
using Serilog;
using Serilog.Events;

try
{
    // configure logger (Serilog)
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    // created builder
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddDefinitions(builder, typeof(Program));

    // create application
    var app = builder.Build();
    app.UseDefinitions();

    // start application
    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}

finally
{
    Log.CloseAndFlush();
}