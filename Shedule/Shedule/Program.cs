using System;

namespace Shedule
{
    public enum Lessons
    {
        Mathematic,
        Physic,
        Informatic
    }
    class Program
    {
        static void Main()
        {   
            List<Lessons> StepanLessons = new List<Lessons>();
            StepanLessons.Add(Lessons.Informatic);
            StepanLessons.Add(Lessons.Mathematic);
            var Stepan = new Teacher("Stepan",StepanLessons, 1);
            Console.WriteLine(Stepan.ToString());
            var Kirill = new Teacher("Kirill", 
                new List<Lessons> { Lessons.Mathematic, Lessons.Informatic },
                1);
            Console.WriteLine(Kirill.ToString());
            var AlexVyach = new Teacher("Aleksandr Vyacheslavovich",
                new List<Lessons> { Lessons.Mathematic, Lessons.Informatic, Lessons.Physic },
                10);
            Console.WriteLine(AlexVyach.ToString());
        }
    }
}
