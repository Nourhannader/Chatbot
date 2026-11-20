using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.Services.CleanUp;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Ef.Services;
using chatbot.Ef.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace chatbot.Ef.Jobs
{
    public class OrphanFileCleanupService(ILogger<OrphanFileCleanupService> logger,IStorageService storageService,IUnitOfWork unitOfWork) : IOrphanFileCleanupService
    {
        private const int BatchSize = 100;
        private const int RetentionDays = 7;
        public async Task CleanupAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);

            var files = await unitOfWork.StoredFiles
                .GetFilesForCleanupAsync(cutoff, BatchSize);

            if (files.Count == 0)
            {
                logger.LogInformation("No files found for cleanup.");

                return;
            }

            var deletedCount = 0;
            var failedCount = 0;

            foreach (var file in files)
            {
                try
                {
                    // Delete physical file from storage.
                    await storageService.DeleteAsync(file);

                    // Mark as physically deleted.
                    file.IsPhysicallyDeleted = true;

                    unitOfWork.StoredFiles.Update(file);

                    deletedCount++;
                }
                catch (Exception ex)
                {
                    failedCount++;

                    logger.LogError(ex,"Failed to delete physical file {FileId}", file.Id);
                }
            }

            // Save successful operations only.
            if (deletedCount > 0)
            {
                await unitOfWork.SaveChangesAsync();
            }

            logger.LogInformation("File cleanup completed. Deleted: {Deleted}, Failed: {Failed}", deletedCount, failedCount);
        }
    }
}
