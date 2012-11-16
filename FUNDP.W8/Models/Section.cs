using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNDP.W8.Models
{
    public class Section
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }
    }
}
