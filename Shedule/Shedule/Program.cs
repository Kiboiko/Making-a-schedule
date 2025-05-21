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

            Student Roman = new Student("Roman", 3, Lessons.Mathematic);
            Console.WriteLine(Roman.ToString());

            Student stud1 = new Student("stud1", 3, Lessons.Physic);
            Console.WriteLine(stud1.ToString());

            Student stud2 = new Student("stud2", 3, Lessons.Informatic);
            Console.WriteLine(stud2.ToString());

            SearchForTeachers(new List<Teacher> { Stepan,Kirill,AlexVyach},
                              new List<Student> { veronica,Roman,stud1,stud2 });
        }
        public static void SearchForTeachers(List<Teacher> teachers, List<Student>students)
        {
            List<Teacher> Ans = new List<Teacher>();
            Dictionary<Lessons,int> PersonPerLesson = new Dictionary<Lessons, int>();
            foreach (Student student in students)
            {
                if (!PersonPerLesson.ContainsKey(student.Lessons))
                {
                    PersonPerLesson[student.Lessons] = 1;
                } else
                {
                    PersonPerLesson[student.Lessons] += 1;
                }
            }
            foreach (Lessons lesson in PersonPerLesson.Keys)
            {
                Console.WriteLine($"{lesson} - {PersonPerLesson[lesson]}");
            }

            int minCountOfTeachers = 0;
            List <Teacher> minCountTeachers = new List<Teacher>();
            foreach ()
        }
    }
}
