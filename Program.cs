using System.Diagnostics;
using Microsoft.FeatureManagement;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ExportProcessorType = OpenTelemetry.ExportProcessorType;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddSource("Microsoft.FeatureManagement");
        tracing.AddJaegerExporter(jaeger =>
        {
            jaeger.Endpoint = new Uri("http://192.168.88.239:14268/api/traces");
            jaeger.ExportProcessorType = ExportProcessorType.Simple;
            jaeger.AgentHost = "192.0.0.1";
            jaeger.AgentPort = 6831;
            jaeger.Protocol = JaegerExportProtocol.UdpCompactThrift;
        });
        tracing.AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddPrometheusExporter();
        metrics.AddMeter("Microsoft.AspNetCore.Hosting");
        metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
        metrics.AddMeter("Microsoft.FeatureManagement");
        metrics.AddAspNetCoreInstrumentation();
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services
    .AddFeatureManagement();

ActivitySource.AddActivityListener(new ActivityListener()
{
    ShouldListenTo = (activitySource) => activitySource.Name == "Microsoft.FeatureManagement",
    Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
    ActivityStopped = (activity) =>
    {
        ActivityEvent? evaluationEvent = activity.Events.FirstOrDefault((activityEvent) => activityEvent.Name == "FeatureFlag");

        if (evaluationEvent.HasValue && evaluationEvent.Value.Tags.Any())
        {
            activity.SetCustomProperty("Tags", evaluationEvent.Value.Tags);
        }
    }
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(swagger =>
    {
        swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        swagger.RoutePrefix = string.Empty;
        swagger.DisplayOperationId();
        swagger.DisplayRequestDuration();
    });
    app.UseOpenTelemetryPrometheusScrapingEndpoint();
}

app.MapDefaultControllerRoute();
app.UseHttpsRedirection();
app.Run();