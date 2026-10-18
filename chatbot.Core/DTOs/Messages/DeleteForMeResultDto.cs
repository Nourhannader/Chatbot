using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.DTOs.Messages
{
    public class DeleteForMeResultDto
    {
        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DeletedAt { get; set; }
    }
}
