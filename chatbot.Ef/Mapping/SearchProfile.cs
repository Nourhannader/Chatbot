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
    public class SearchProfile:Profile
    {
        public SearchProfile()
        {
            CreateMap<ApplicationUser, UserSearchDto>();
            CreateMap<Message,MessageSearchDto>();
            CreateMap<Conversation, ConversationSearchDto>();
            CreateMap<StoredFile, FileSearchDto>();
        
        }
    }
}
