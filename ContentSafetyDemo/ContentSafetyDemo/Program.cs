using Azure.Identity;
using ContentSafetyDemo.Components;
using ContentSafetyDemo.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

string? cs = Environment.GetEnvironmentVariable("RecExtConfigConnectionString");
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(cs)
            .ConfigureKeyVault(kv =>
            {
                kv.SetCredential(new DefaultAzureCredential());
            });
});

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddContentSafetyClient(new Uri(builder.Configuration["contentSafetyEndpoint"]),
        new Azure.AzureKeyCredential(builder.Configuration["contentSafetyKey"]));
    clientBuilder.AddBlocklistClient(new Uri(builder.Configuration["contentSafetyEndpoint"]),
        new Azure.AzureKeyCredential(builder.Configuration["contentSafetyKey"]));
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddTransient<ContentSafetyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
