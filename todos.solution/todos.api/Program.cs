using Microsoft.EntityFrameworkCore;
using todos.api.Common;
using todos.api.Data;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "todos.api", Version = "v1" });
});

// Use Serilog for logging and read configuration from appsettings
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

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
// Add services to the container.

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();
