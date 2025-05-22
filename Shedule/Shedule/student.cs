using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public class Student : Person
    {
        public Lessons Subject { get;}

        public Student(string name, string startOfStudyTime, string endOfStudyTime,Lessons subject) 
            : base(name, startOfStudyTime, endOfStudyTime)
        {
            Subject = subject;
        }

        public override string ToString()
        {
            return $"Класс: Ученик\nИмя: {Name} \nВремя начала:{StartOfStudyingTime.ToString("HH:mm")}\n" +
                $"Время конца:{EndOfStudyingTime.ToString("HH:mm")} \nПредмет:{Subject}";
        }
    }
}
