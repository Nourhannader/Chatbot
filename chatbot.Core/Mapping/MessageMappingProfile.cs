using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Messages;
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


            // Reaction
            CreateMap<MessageReaction, MessageReactionDto>()
                .ForMember(
                    d => d.UserName,
                    opt => opt.MapFrom(
                        s => s.User.UserName));


            // File
            CreateMap<StoredFile, FileMetadataDto>()
                .ForMember(
                    d => d.FileUrl,
                    opt => opt.MapFrom(
                        s => s.FileUrl));


            // Reply preview
            CreateMap<Message, ReplyPreviewDto>()
                .ForMember(
                    d => d.SenderName,
                    opt => opt.MapFrom(
                        s => s.Sender.UserName))
                .ForMember(
                    d => d.Content,
                    opt => opt.MapFrom(
                        s => s.IsDeletedForEveryone
                            ? null
                            : s.Content));


            // Main Message
            CreateMap<Message, MessageDto>()
                .ForMember(
                    d => d.SenderName,
                    opt => opt.MapFrom(
                        s => s.Sender.UserName))
                .ForMember(
                    d => d.CreatedAt,
                    opt => opt.MapFrom(
                        s => s.SendAt))
                .ForMember(
                  d => d.attachments,
                  opt => opt.MapFrom(src  =>
                  src.Attachments.OrderBy(x =>x.Order)))

                .ForMember(
                d => d.VoiceNote,
                opt => opt.Ignore())
                .ForMember(
                    d => d.Content,
                    opt => opt.MapFrom(
                        s => s.IsDeletedForEveryone
                            ? null
                            : s.Content));


            CreateMap<MessageAttachment, FileMetadataDto>()

    .ForMember(
        d => d.Id,
        opt => opt.MapFrom(
            s => s.StoredFile.Id))

    .ForMember(
        d => d.OriginalName,
        opt => opt.MapFrom(
            s => s.StoredFile.OriginalFileName))

    .ForMember(
        d => d.ContentType,
        opt => opt.MapFrom(
            s => s.StoredFile.ContentType))

    .ForMember(
        d => d.Size,
        opt => opt.MapFrom(
            s => s.StoredFile.FileSize))

    .ForMember(
        d => d.FileUrl,
        opt => opt.MapFrom(
            s => s.StoredFile.FileUrl))

    .ForMember(
        d => d.Width,
        opt => opt.MapFrom(
            s => s.StoredFile.Width))

    .ForMember(
        d => d.Height,
        opt => opt.MapFrom(
            s => s.StoredFile.Height));

            CreateMap<MessageAttachment, MessageAttachmentDto>()
    .ForMember(
        dest => dest.File,
        opt => opt.MapFrom(src => src.StoredFile));
        }

    }
}
