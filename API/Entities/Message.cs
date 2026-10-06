using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Constraints;

namespace API.Entities
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Content { get; set; }
        public DateTime SentDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? ReadDateTime { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        
        // nav properties
        public required string SenderId { get; set; }
        public Member Sender { get; set; } = null!;
        public required string RecipientId { get; set; }
        public Member Recipient { get; set; } = null!;
    }
}