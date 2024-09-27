using Microsoft.Extensions.Options;
using todos.service.Common;
using todos.service.Data;
using todos.service.Models;

namespace todos.service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher _watcher;
        private string _sourcePath;
        private string _destinationPath;
        private readonly AppDbContext _dbContext;
        private readonly AppSettings _appSettings;

        public Worker(ILogger<Worker> logger, AppDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _dbContext = dbContext;
            _sourcePath = appSettings.Value.FileDirectory.Source;
            _destinationPath = appSettings.Value.FileDirectory.Destination;
            _appSettings = appSettings.Value;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Service is starting with source path: {_sourcePath} and destination path: {_destinationPath}");

            if (!Directory.Exists(_sourcePath))
            {
                Directory.CreateDirectory(_sourcePath);
                _logger.LogInformation($"Source directory created: {_sourcePath}");
            }

            _watcher = new FileSystemWatcher(_sourcePath)
            {
                Filter = "*.*",  // Monitor all files
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite  // Notify on new files
            };
            _watcher.Created += OnCreated;
            _watcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"File created: {e.FullPath}");

            try
            {
                // Wait briefly to ensure the file is ready
                await Task.Delay(1000);

                // Read the content of the file from the source folder
                string fileContent = await File.ReadAllTextAsync(e.FullPath);
                string title = ExtractTitle(fileContent);

                if (!string.IsNullOrEmpty(title))
                {
                    // Insert the extracted Title into the database
                    var todo = new Todo
                    {
                        Title = title
                    };

                    _dbContext.Todos.Add(todo);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Title '{title}' saved to the database.");

                    // After inserting into the database, move the file to the destination folder
                    var destinationFile = Path.Combine(_destinationPath, Path.GetFileName(e.FullPath));
                    File.Move(e.FullPath, destinationFile);

                    _logger.LogInformation($"File moved: {e.FullPath} -> {destinationFile}");
                }
                else
                {
                    _logger.LogWarning($"Title not found in the file: {e.FullPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing file: {e.FullPath}");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service is stopping.");

            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();

            return base.StopAsync(cancellationToken);
        }

        private string ExtractTitle(string fileContent)
        {
            // Assuming the file has a format like "Title: some_value"
            var titleLine = fileContent.Split('\n').FirstOrDefault(line => line.StartsWith("Title:"));
            return titleLine?.Replace("Title:", "").Trim();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("Service is stopping.");
        }
    }
}
