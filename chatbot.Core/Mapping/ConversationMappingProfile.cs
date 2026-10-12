using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.InviteGroup;
using chatbot.Core.Models;

namespace chatbot.Core.Mapping
{
    public class ConversationMappingProfile:Profile
    {
        public ConversationMappingProfile()
        {
            CreateMap<Conversation, ConversationDetailsDto>()
                .ForMember(d => d.MembersCount, opt => opt.MapFrom(s => s.Members.Count(m => m.LeftAt == null && m.IsBanned)))
                .ForMember(d=> d.IsMuted, opt => opt.Ignore())
                .ForMember(d=> d.IsPinned,opt => opt.Ignore())
                .ForMember(d => d.IsArchived,opt => opt.Ignore());
            CreateMap<ConversationMember, ConversationMemberDetailsDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.UserName))
                .ForMember(d => d.DisplayName, opt => opt.MapFrom(s => s.User.FirstName + s.User.LastName))
                .ForMember(d => d.ProfileImageUrl, opt => opt.MapFrom(s => s.User.ProfileImage != null ? s.User.ProfileImage.Path : null));
            CreateMap<ConversationMember, ConversationMemberDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.UserName))
                .ForMember(d => d.DisplayName, opt => opt.MapFrom(s => s.User.FirstName + s.User.LastName))
                .ForMember(d => d.ProfileImageUrl, opt => opt.MapFrom(s => s.User.ProfileImage != null ? s.User.ProfileImage.Path : null));


            CreateMap<ConversationUserSettings, ConversationSettingsDto>();
            CreateMap<GroupInvite, GroupInviteDto>();
            CreateMap<StoredFile, ConversationImageDto>()
                .ForMember(
                d => d.FielId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Url,
                opt => opt.MapFrom(s => s.Path));

        }
    }
}
