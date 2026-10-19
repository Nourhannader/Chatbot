using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.Notifications;
using chatbot.Core.Models;

namespace chatbot.Core.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification,NotificationDto>();

            CreateMap<NotificationPreferences, NotificationPreferencesDto>();

            CreateMap< UpdateNotificationPreferencesDto,NotificationPreferences>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.Ignore())
                .ForMember(
                    dest => dest.User,
                    opt => opt.Ignore());


            CreateMap<UserDevice,UserDeviceDto>();
        }
    }
}
