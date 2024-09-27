using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using todos.service;
using todos.service.Common;
using todos.service.Data;

var builder = Host.CreateApplicationBuilder(args);

// Add environment variables to the configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); // Add this last to ensure env vars override appsettings.json

// Bind AppSettings
var appSettings = new AppSettings();
builder.Configuration.Bind("AppSettings", appSettings);

// Make sure to register AppSettings in the DI container if needed
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

string dbConnectionString = string.Empty;

try
{
    var credentialOptions = new ClientSecretCredentialOptions
    {
        AdditionallyAllowedTenants = { "*" }
    };

    var credential = new ClientSecretCredential(appSettings.AzureCredentials.TenantId,
        appSettings.AzureCredentials.ClientId,
        appSettings.AzureCredentials.ClientSecret);

    var client = new SecretClient(new Uri(appSettings.AzureKeyVault.KeyVaultUrl), credential);

    KeyVaultSecret todoDbSecret = client.GetSecret(appSettings.AzureKeyVault.TodoDbSecretName);

    dbConnectionString = todoDbSecret.Value;
}
catch (Exception ex)
{
    // Log the exception if needed
    Console.WriteLine($"Failed to get the connection string from KeyVault: {ex.Message}");
    dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}

// Register the DbContext and configure it with the connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(dbConnectionString));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
