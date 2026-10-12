using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Core.Mapping
{
    public class MessageMappingProfile:Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<Message, LastMessageDto>()
                .ForMember(d => d.SenderName,
                opt => opt.MapFrom(s => s.Sender.UserName));
        }
    }
}
