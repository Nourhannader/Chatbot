using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IForwardService
    {
        Task<List<Message>> ForwardAsync( string senderId,ForwardMessageDto dto);
    }
}
