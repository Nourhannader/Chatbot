using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Background
{
    public class FileCleanupBackgroundService : BackgroundService
    {
        private readonly ILogger<FileCleanupBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FileCleanupBackgroundService(
            ILogger<FileCleanupBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("File Cleanup Background Service started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupFilesAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                 when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error occurred while cleaning files.");
                }
                try
                {
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            _logger.LogInformation("File Cleanup Background Service stopped.");
        }
        private async Task CleanupFilesAsync(
        CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var providers = scope.ServiceProvider.GetServices<IStorageProvider>();
            var olderThan = DateTime.UtcNow.AddDays(-30);
            var filesToCleanup = await unitOfWork.StoredFiles.GetFilesForCleanupAsync(olderThan);

            if (!filesToCleanup.Any())
            {
                _logger.LogInformation("No files to cleanup.");
                return;
            }

            foreach (var file in filesToCleanup)
            {
                stoppingToken.ThrowIfCancellationRequested();

                try
                {
                    var provider = providers.FirstOrDefault(p => p.ProviderType == file.StorageProvider);
                    if (provider == null)
                    {
                        _logger.LogError(
                            "Storage provider {Provider} not found for file {FileId}",
                            file.StorageProvider,
                            file.Id);
                        continue;
                    }
                    await provider.DeleteAsync(file.Path, stoppingToken);
                    file.IsPhysicallyDeleted = true;
                    unitOfWork.StoredFiles.UpdateAsync(file);
                    _logger.LogInformation(
                    "File {FileId} was physically deleted.",
                    file.Id);

                }
                catch (Exception ex)
                {
                    _logger.LogError(
                          ex,
                         "Error deleting file {FileId} from storage provider {Provider}",
                          file.Id,
                        file.StorageProvider);
                }

            }
            unitOfWork.SaveChangesAsync();
        }
    }
}
