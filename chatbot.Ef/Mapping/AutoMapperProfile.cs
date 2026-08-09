using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.Models;

namespace chatbot.Ef.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<Message, MessageDto>();

            CreateMap<ApplicationUser, UserDto>();

            CreateMap<Conversation, ConversationDto>();

            CreateMap<MessageReaction, MessageReactionDto>();

            CreateMap<Notification, NotificationDto>();
        }
    }
}
