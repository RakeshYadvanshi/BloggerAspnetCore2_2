using System;

namespace BloggerAPI.DTO.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }
        public int CreatedBy { get; set; }
    }
}
