using Microsoft.AspNetCore.Localization;
using PortfelInwestycji.Server.Assets;
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
builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        // Binding failures (e.g. a tampered select value) reach the user too, so say them in Polish.
        var messages = options.ModelBindingMessageProvider;
        messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"Wartość „{value}” w polu {field} jest nieprawidłowa.");
        messages.SetValueIsInvalidAccessor(value => $"Wartość „{value}” jest nieprawidłowa.");
        messages.SetValueMustNotBeNullAccessor(field => $"Pole {field} jest wymagane.");
        messages.SetMissingBindRequiredValueAccessor(field => $"Brak wartości pola {field}.");
        messages.SetUnknownValueIsInvalidAccessor(field => $"Wartość w polu {field} jest nieprawidłowa.");
        messages.SetNonPropertyAttemptedValueIsInvalidAccessor(value => $"Wartość „{value}” jest nieprawidłowa.");
        messages.SetNonPropertyUnknownValueIsInvalidAccessor(() => "Podana wartość jest nieprawidłowa.");
        messages.SetNonPropertyValueMustBeANumberAccessor(() => "Wpisz liczbę.");
        messages.SetValueMustBeANumberAccessor(field => $"Pole {field} musi być liczbą.");
        messages.SetMissingKeyOrValueAccessor(() => "Brak wartości.");
        messages.SetMissingRequestBodyRequiredValueAccessor(() => "Brak treści żądania.");
    });

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<AssetStore>();
builder.Services.AddSingleton<AssetService>();

var app = builder.Build();

// Fail at startup, not on the first request, if the assets file cannot be read.
await app.Services.GetRequiredService<AssetStore>().LoadAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// Polish only, regardless of the browser's Accept-Language.
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(Amount.Culture),
    SupportedCultures = [Amount.Culture],
    SupportedUICultures = [Amount.Culture],
    RequestCultureProviders = [],
});
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapOpenApi();
app.MapScalarApiReference();
app.MapRazorPages();

await app.RunAsync();
