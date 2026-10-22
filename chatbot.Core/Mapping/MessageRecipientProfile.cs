using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.MessageStatus;
using chatbot.Core.Enums;
using chatbot.Core.Models;

namespace chatbot.Core.Mapping
{
    public class MessageRecipientProfile : Profile
    {
        public MessageRecipientProfile()
        {
            CreateMap<MessageRecipientStatus, MessageRecipientStatusDto>()
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src =>
                        src.ReadAt.HasValue
                            ? MessageStatus.Read
                            : src.DeliveredAt.HasValue
                                ? MessageStatus.Delivered
                                : MessageStatus.Sent));
        }
    }
}
