using System;
using System.Collections.Generic;
using System.Text;

namespace BloggerAPI.DTO.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public string CommentOn { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
