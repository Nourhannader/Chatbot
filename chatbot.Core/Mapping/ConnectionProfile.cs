using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using chatbot.Core.DTOs.UserConnection;
using chatbot.Core.Models;

namespace chatbot.Core.Mapping
{
    public class ConnectionProfile:Profile
    {
        public ConnectionProfile()
        {
            CreateMap<UserConnection, UserConnectionDto>();
            CreateMap<UserConnectionDto, UserConnection>();
        }
    }
}
