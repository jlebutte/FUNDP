using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNDP.W8.Models
{
    public class Cours
    {
        private static List<string> _week = new List<string>() 
        { 
            "Lundi", 
            "Mardi", 
            "Mercredi", 
            "Jeudi", 
            "Vendredi", 
            "Samedi", 
            "Dimanche" 
        };

        private DateTime _start;

        public string Name { get; set; }
        public string Day { get; set; }
        public string Local { get; set; }
        public string Start
        {
            get { return _start.ToString(CultureInfo.InvariantCulture); }
            set
            {
                _start = DateTime.Parse(value).AddDays(_week.IndexOf(Day));
            }
        }
        public string End { get { return _start.AddHours(2).ToString(CultureInfo.InvariantCulture); } }
        public string Prof { get; set; }
    }
}
