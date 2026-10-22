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
    public class MediaMappingProfile :Profile
    {
        public MediaMappingProfile()
        {
            CreateMap<StoredFile, FileMetadataDto>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.OriginalName,
                    opt => opt.MapFrom(src => src.OriginalFileName))
                .ForMember(
                    dest => dest.ContentType,
                    opt => opt.MapFrom(src => src.ContentType))
                .ForMember(
                    dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category))
                .ForMember(
                    dest => dest.Size,
                    opt => opt.MapFrom(src => src.FileSize))
                .ForMember(
                    dest => dest.Width,
                    opt => opt.MapFrom(src => src.Width))
                .ForMember(
                    dest => dest.Height,
                    opt => opt.MapFrom(src => src.Height))
                .ForMember(
                    dest => dest.DurationSeconds,
                    opt => opt.MapFrom(src => src.DurationSeconds))
                .ForMember(
                    dest => dest.ProcessingStatus,
                    opt => opt.MapFrom(src => src.ProcessingStatus))

                // Generated through StorageService
                .ForMember(
                    dest => dest.FileUrl,
                    opt => opt.Ignore())

                .ForMember(
                    dest => dest.ThumbnailUrl,
                    opt => opt.Ignore());



            CreateMap<VoiceNote, VoiceNoteDto>()
            .ForMember(
                d => d.FileUrl,
                o => o.Ignore())

            .ForMember(
                d => d.Waveform,
                o => o.Ignore())

            .ForMember(
                d => d.ProcessingStatus,
                o => o.MapFrom(
                    s => s.File.ProcessingStatus));
        }
    }
}
