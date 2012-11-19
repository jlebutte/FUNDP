using System;
using System.Collections.Generic;
using System.Globalization;

namespace Schedule.Models
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
        public DateTime DayDT { get { return _start; } }

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

        public string Prof { get; set; }
    }
}