using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    internal class Student
    {

        public string Name {  get; set; }

        public int Time { get; set; }
        public Lessons Lessons { get; set; }

        public Student(string name,int time, Lessons lessons)
        {
            Name = name;
            Time = time;
            Lessons = lessons;
        }

        public override string ToString()
        {
            return $"Имя: {Name} \n Время{Time} \n Предмет{Lessons}";
        }

        public static void test()
        {

        }
    }
}
