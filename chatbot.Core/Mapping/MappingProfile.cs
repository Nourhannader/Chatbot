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
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //Message
            CreateMap<Message, MessageDto>();
            CreateMap<Message,MessageSearchDto>();

            //Conversation
            CreateMap<Conversation, ConversationDto>();
            CreateMap<Conversation, ConversationSearchDto>()
                .ForMember(
                   dest => dest.LastMessageAt,
                   opt => opt.MapFrom(src => src.Messages.OrderBy(m => m.SendAt).Take(1).Select(m  => m.SendAt))
                );

            //ConversationMember
            CreateMap<ConversationMember, ConversationMemberDto>()
                .ForMember(
                   dest => dest.UserName,
                   opt => opt.MapFrom(src => src.User.UserName)
                );
            //notification
            CreateMap<Notification, NotificationDto>();
            
            

        }
    }
}
