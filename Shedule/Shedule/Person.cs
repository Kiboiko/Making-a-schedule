using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public class Person
    {
        public string Name { get;}
        public int StartOfStudyingTime { get;}
        public int EndOfStudyingTime { get; }
        public Person(string name, string startOfStudyTime, string endOfStudyTime)
        {
            Name = name;
            if (TimeOnly.TryParse(startOfStudyTime, out TimeOnly StartOfStudyingTime) && 
                TimeOnly.TryParse(endOfStudyTime, out TimeOnly EndOfStudyingTime))
            {
                Console.WriteLine($"{StartOfStudyingTime.ToString("HH:mm")}   {EndOfStudyingTime.ToString("HH:mm")}");
            } else
            {
                Console.WriteLine("Некорректный формат времени");
            }
        }
    }
}
