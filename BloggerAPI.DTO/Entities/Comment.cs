using System;
using System.Collections.Generic;
using System.Text;

namespace BloggerAPI.DTO.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public int CommentOnId { get; set; }
        public string CommentOn { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }

        public int CreatedBy { get; set; }

        public Comment(string commentText, int commentOnId, string commentOn, DateTime createdDate, DateTime? lastModified, int createdBy)
        {
            CommentText = commentText;
            CommentOnId = commentOnId;
            CommentOn = commentOn;
            CreatedDate = createdDate;
            LastModified = lastModified;
            CreatedBy = createdBy;
        }

        public Comment()
        {
            
        }
        
    }
}
