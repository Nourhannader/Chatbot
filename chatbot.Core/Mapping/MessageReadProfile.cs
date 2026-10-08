using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.MessageRead;
using chatbot.Core.Models;

namespace chatbot.Core.Mapping
{
    public class MessageReadProfile:Profile
    {
        public MessageReadProfile()
        {
            CreateMap<MessageRead, MessageReadDto>();
        }
    }
}
