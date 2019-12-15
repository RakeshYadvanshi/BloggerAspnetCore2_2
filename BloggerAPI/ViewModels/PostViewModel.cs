using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAPI.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string PostTitle { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
