using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Interfaces.Services;
using chatbot.Core.Interfaces.UnitOFWork;
using chatbot.Core.Models;
using chatbot.Ef.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace chatbot.Ef.Services
{
    public class ChunkUploadService(IUnitOfWork unitOfWork) : IChunkUploadService
    {
        public async Task<UploadResultDto> CompleteAsync(Guid sessionId, Guid messageId, Guid userId, CancellationToken cancellationToken = default)
        {
            var session =await unitOfWork.UploadSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                throw new KeyNotFoundException("Upload session not found.");
            }
            if (session.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to upload to this session.");
            }
            if(session.UploadedChunks != session.TotalChunks)
            {
                throw new InvalidOperationException("Not all chunks have been uploaded.");
            }
            var folder= Path.Combine("uploads", "chunks", sessionId.ToString());

            var chunkFiles=Directory.GetFiles(folder, "*.chunk")
                       .OrderBy(x =>int.Parse(Path.GetFileNameWithoutExtension(x))).ToList();

            var tempFilePath = Path.Combine(Path.GetTempPath(),
                  $"{Guid.NewGuid()}_{session.OriginalFileName}");

            await using (var output = new FileStream(tempFilePath,FileMode.Create,FileAccess.Write,FileShare.None,81920,useAsync: true))
            {
                foreach (var chunkPath in chunkFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await using var input = new FileStream(chunkPath,FileMode.Open,FileAccess.Read,FileShare.Read,81920,useAsync: true);

                    await input.CopyToAsync( output,cancellationToken);
                }
            }

            session.IsCompleted = true;
            session.CompletedAt = DateTime.UtcNow;

            unitOfWork.UploadSessions.Update(session);

            await unitOfWork.SaveChangesAsync();
                

            Directory.Delete(folder, true);

            throw new NotImplementedException("Next: Save merged file through StorageService.");

        }

        public async Task<Guid> StartAsync(StartUploadDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var session = new UploadSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OriginalFileName = dto.FileName,
                ContentType = dto.ContentType,
                TotalChunks = dto.TotalChunks,
                TotalSize = dto.TotalSize,
                UploadedChunks = 0,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            await unitOfWork.UploadSessions.AddAsync(session);

            await unitOfWork.SaveChangesAsync();

            return session.Id;
        }

        public async Task UploadChunkAsync(Guid sessionId, int chunkNumber, IFormFile chunk, Guid userId, CancellationToken cancellationToken = default)
        {
            var session = await unitOfWork.UploadSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                throw new KeyNotFoundException("Upload session not found.");
            }
            if (session.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to upload to this session.");
            }
            if(session.IsCompleted)
            {
                throw new InvalidOperationException("Upload session is already completed.");
            }
            if(chunkNumber <0 || chunkNumber >= session.TotalChunks)
            {
                throw new ArgumentException("Invalid chunk number.");
            }

            var folder=Path.Combine("uploads","chunks", sessionId.ToString());
            Directory.CreateDirectory(folder);
            var chunkPath= Path.Combine(folder, $"{chunkNumber}.Number");
            await using var stream = new FileStream(chunkPath,FileMode.Create,FileAccess.Write,FileShare.None,81920,useAsync: true);

            await chunk.CopyToAsync(stream,cancellationToken);
            session.UploadedChunks++;

            unitOfWork.UploadSessions.Update(session);

            await unitOfWork.SaveChangesAsync();

        }
    }
}
