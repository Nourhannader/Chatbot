using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Models
{
    public class StoredFile : BaseEntity
    {
        public string OriginalName { get; set; }= null!;
        public string StoredName {  get; set; }= null!;
        public string Path {  get; set; }= null!;
        public string ContentType {  get; set; }= null!;
        public long Size { get; set; }
        public bool IsDeleted {  get; set; } = false;
        public DateTime CreatedAt {  get; set; }= DateTime.Now;
        public Guid UploadedByUserId { get; set;} 
        public ApplicationUser UploadedByUser { get; set; } = null!;
        public Guid? MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
