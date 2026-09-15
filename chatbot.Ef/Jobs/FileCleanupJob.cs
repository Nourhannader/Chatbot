using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.UnitOfWork;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public class FileCleanupJob
    {
        private readonly ILogger<FileCleanupJob> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEnumerable<IStorageProvider> providers;

        public FileCleanupJob(
            ILogger<FileCleanupJob> logger,
            IUnitOfWork unitOfWork,
            IEnumerable<IStorageProvider> providers)
        {
            logger = logger;
            unitOfWork = unitOfWork;
            providers = providers;
        }
        [AutomaticRetry(Attempts = 3)]
        public async Task ExecuteAsync()
        {
            logger.LogInformation(
                "File cleanup job started.");

            var olderThan =
                DateTime.UtcNow.AddDays(-30);
            var filesToCleanup =
                await unitOfWork.StoredFiles
                    .GetFilesForCleanupAsync(olderThan);

            if (!filesToCleanup.Any())
            {
                logger.LogInformation(
                    "No files to cleanup.");

                return;
            }

            foreach (var file in filesToCleanup)
            {
                try
                {
                    var provider =
                        providers.FirstOrDefault(
                            p => p.ProviderType == file.Provider);

                    if (provider == null)
                    {
                        logger.LogError(
                            "Storage provider {Provider} not found for file {FileId}",
                            file.Provider,
                            file.Id);

                        continue;
                    }

                    await provider.DeleteAsync(
                        file.Path,
                        CancellationToken.None);
                    file.IsPhysicallyDeleted = true;

                    await unitOfWork.StoredFiles
                        .UpdateAsync(file);

                    logger.LogInformation(
                        "File {FileId} was physically deleted.",
                        file.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Error deleting file {FileId} from storage provider {Provider}",
                        file.Id,
                        file.Provider);
                }
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation(
                "File cleanup job completed.");
        }
    }
}
