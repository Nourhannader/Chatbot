using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Messages
{
    public class MessageEditedDto
    {
        public Guid MessageId { get; set; }
        public string Content {  get; set; }
        public int Version {  get; set; }
        public DateTime EditedAt {  get; set; }
    }
}
