using System;
using System.Collections.Generic;
using System.Text;

namespace BloggerAPI.DTO.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string PostTitle { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }

        public Post(string postTitle, string shortDescription, string description, int createdBy, DateTime createdDate)
        {
            PostTitle = postTitle;
            ShortDescription = shortDescription;
            Description = description;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }

        public Post()
        {
            
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Description))
            {
                return $"{nameof(Id)}: {Id}, {nameof(PostTitle)}: {PostTitle}, {nameof(ShortDescription)}: {ShortDescription}, {nameof(Description)}: {Description}, {nameof(CreatedBy)}: {CreatedBy}, {nameof(CreatedDate)}: {CreatedDate}, {nameof(LastModified)}: {LastModified}";

            }

            return "";
        }
    }
}
