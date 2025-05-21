using System;
using System.Collections.Generic;

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
            //Console.WriteLine(Stepan.ToString());
            var Kirill = new Teacher("Kirill", 
                new List<Lessons> { Lessons.Mathematic, Lessons.Informatic },
                1);
            //Console.WriteLine(Kirill.ToString());

            var AlexVyach = new Teacher("Aleksandr Vyacheslavovich",
                new List<Lessons> { Lessons.Mathematic, Lessons.Informatic, Lessons.Physic },
                10);
            //Console.WriteLine(AlexVyach.ToString());
            
            Student veronica = new Student("Veronica", 3, Lessons.Mathematic);
           // Console.WriteLine(veronica.ToString());

            Student Roman = new Student("Roman", 3, Lessons.Mathematic);
            //Console.WriteLine(Roman.ToString());

            Student stud1 = new Student("stud1", 3, Lessons.Physic);
            //Console.WriteLine(stud1.ToString());

            Student stud2 = new Student("stud2", 3, Lessons.Informatic);
            //Console.WriteLine(stud2.ToString());

            /*SearchForTeachers(new List<Teacher> { Stepan,Kirill,AlexVyach},
                              new List<Student> { veronica,Roman,stud1,stud2 });*/

            var uniqListTeacher = GetAllTeacherCombinations(new List<Teacher> { Stepan, Kirill, AlexVyach });
            foreach (var Combination in uniqListTeacher)
            {
                Console.WriteLine(String.Join(' ',Combination.Select(x=>x.Name)));
            }
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
            var uniqListTeacher = GetAllTeacherCombinations(teachers);
        }

        public static List<List<Teacher>> GetAllTeacherCombinations(List<Teacher> teachers)
        {
            var result = new List<List<Teacher>>();
            GenerateCombinations(teachers, 0, new List<Teacher>(), result);
            return result;
        }

        private static void GenerateCombinations(
        List<Teacher> teachers,
        int index,
        List<Teacher> current,
        List<List<Teacher>> result)
        {
            if (index == teachers.Count)
            {
                result.Add(new List<Teacher>(current));
                return;
            }

            // Включаем текущего учителя
            current.Add(teachers[index]);
            GenerateCombinations(teachers, index + 1, current, result);
            current.RemoveAt(current.Count - 1); // Backtrack

            // Не включаем текущего учителя
            GenerateCombinations(teachers, index + 1, current, result);
        }
    }
}
