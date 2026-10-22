using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Ef.Repositories
{
    class VoiceNoteRepository(ApplicationDbContext context) : IVoiceNoteRepository
    {
        public async Task AddAsync(VoiceNote entity)
        {
          await context.VoiceNotes.AddAsync(entity);
        }

        public async Task<VoiceNote?> GetByIdAsync(Guid id)
        {
          return  await  context.VoiceNotes
                .Include(vn=> vn.Message)
                .Include(vn => vn.File)
                .FirstOrDefaultAsync(vn => vn.Id == id);
        }

        public async Task<VoiceNote?> GetByMessageIdAsync(Guid messageId)
        {
            return await context.VoiceNotes
                .Include(x => x.File)
                .FirstOrDefaultAsync(
                    x => x.MessageId == messageId);
        }

        public async Task<VoiceNote?> GetByFileIdAsync(Guid fileId)
        {
            return await context.VoiceNotes
                .Include(x => x.File)
                .FirstOrDefaultAsync(
                    x => x.FileId == fileId);
        }

        public async Task<VoiceNote?> GetForProcessingAsync(Guid voiceNoteId)
        {
            return await context.VoiceNotes
                .Include(x => x.File)
                .Include(x => x.Message)
                .FirstOrDefaultAsync(
                    x => x.Id == voiceNoteId);
        }

        public void Remove(VoiceNote voiceNote)
        {
            context.VoiceNotes.Remove(voiceNote);
        }

        public void Update(VoiceNote entity)
        {
            context.VoiceNotes.Update(entity);
        }
    }
}
