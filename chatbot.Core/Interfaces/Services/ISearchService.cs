using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using Microsoft.VisualBasic.FileIO;

namespace chatbot.Core.Interfaces.Services
{
    public interface ISearchService
    {
        Task<SearchResultDto> SearchAsync(Guid userId, SearchDto dto);
    }
}
