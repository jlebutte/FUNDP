using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Schedule.Models
{
    public class Section
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }
    }
}