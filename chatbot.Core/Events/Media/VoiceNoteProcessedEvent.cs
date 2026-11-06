using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Events;

namespace chatbot.Core.Events.Media
{
    public sealed record VoiceNoteProcessedEvent(Guid MessageId, Guid ConversationId, Guid VoiceNoteId,
    double DurationSeconds, string Waveform, DateTime ProcessedAt) : DomainEvent;
}
