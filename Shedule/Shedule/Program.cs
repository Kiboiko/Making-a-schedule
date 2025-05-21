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
            var Stepan = new Teacher("Stepan", 
                new List<Lessons> { Lessons.Mathematic, Lessons.Informatic },
                1);
            Console.WriteLine(Stepan.ToString());
            var Kirill = new Teacher("Kirill", 
                new List<Lessons> { Lessons.Mathematic, Lessons.Informatic },
                1);
            Console.WriteLine(Kirill.ToString());
            var AlexVyach = new Teacher("Aleksandr Vyacheslavovich",
                new List<Lessons> { Lessons.Mathematic, Lessons.Informatic, Lessons.Physic },
                10);
            Console.WriteLine(AlexVyach.ToString());

            Student veronica = new Student("Veronica", 3, Lessons.Mathematic);
            Console.WriteLine(veronica.ToString());
        }
    }
}
