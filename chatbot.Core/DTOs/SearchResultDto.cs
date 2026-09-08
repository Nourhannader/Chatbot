using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Enums;

namespace chatbot.Core.DTOs
{
    public class SearchResultDto
    {
        public SearchType Type { get; set; }
        public int TotalCount {  get; set; }
        public int PageNumber {  get; set; }
        public int PageSize {  get; set; }
        public object Data { get; set; } = new();
    }
}
