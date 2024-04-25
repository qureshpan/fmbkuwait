using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMB_Kuwait.Models
{
    public class TagDetail
    {
        public int MemberId { get; set; }
        public List<Tag> tags { get; set; }
        public Tag tag { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public TagDetail()
        {
            Message = string.Empty;
        }
    }
    public class Tag
    {
        public int TagId { get; set; }
        public string TagName { get; set; }

    }
}