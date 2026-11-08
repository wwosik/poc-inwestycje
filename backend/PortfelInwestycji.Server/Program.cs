using PortfelInwestycji.Server.Configuration;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var appPaths = AppPaths.Resolve(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services.AddSingleton(appPaths);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(appPaths.LogsDirectory, "log-.txt"),
        rollingInterval: RollingInterval.Day));

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapOpenApi();
app.MapScalarApiReference();
app.MapRazorPages();

app.Run();
