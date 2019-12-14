using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAPI.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public string CommentOn { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
