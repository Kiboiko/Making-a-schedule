using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Shedule
{
    public enum Lessons
    {
        Math,
        Physic,
        Informatic
    }
    class Program
    {
        static void Main()
        {
            var Stepan = new Teacher("Stepan", "14:00", "20:00",
                new List<Lessons> { Lessons.Math, Lessons.Informatic }, 1);
            Console.WriteLine(Stepan.ToString());

            var Kirill = new Teacher("Kirill", "15:00", "19:00",
                new List<Lessons> { Lessons.Math, Lessons.Informatic }, 1);
            Console.WriteLine(Kirill.ToString());
            var Alexandr = new Teacher("Alxandr", "12:00", "21:00",
                new List<Lessons> { Lessons.Math, Lessons.Informatic, Lessons.Physic }, 10);
            Console.WriteLine(Alexandr.ToString());

            var stud1 = new Student("Veronica","18:00","20:00",Lessons.Math);
            var stud2 = new Student("Roman", "16:00", "18:00", Lessons.Informatic);
            var stud3 = new Student("Nikita", "17:00", "20:00", Lessons.Math);
            var stud4 = new Student("Arthur", "14:00", "17:00", Lessons.Physic);
            Console.WriteLine($"{stud1.ToString()}\n" +
                $"{stud2.ToString()}\n" +
                $"{stud3.ToString()}\n" +
                $"{stud4.ToString()}\n");

        }
        public static void SearchForTeachers(List<Teacher> teachers, List<Student>students)
        {
            List<Teacher> Ans = new List<Teacher>();
            Dictionary<Lessons,int> PersonPerLesson = new Dictionary<Lessons, int>();
            foreach (Student student in students)
            {
                if (!PersonPerLesson.ContainsKey(student.Subject))
                {
                    PersonPerLesson[student.Subject] = 1;
                } else
                {
                    PersonPerLesson[student.Subject] += 1;
                }
            }
            foreach (Lessons lesson in PersonPerLesson.Keys)
            {
                Console.WriteLine($"{lesson} - {PersonPerLesson[lesson]}");
            }

            int minCountOfTeachers = 0;
            List <Teacher> minCountTeachers = new List<Teacher>();
            var uniqListTeacher = GetAllTeacherCombinations(teachers);
            foreach (var Combination in uniqListTeacher)
            {
                if (ClosureOfNeeds(Combination,PersonPerLesson))
                    Console.WriteLine(String.Join(' ', Combination.Select(x => x.Name)));
            }
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

            current.Add(teachers[index]);
            GenerateCombinations(teachers, index + 1, current, result);
            current.RemoveAt(current.Count - 1);

            GenerateCombinations(teachers, index + 1, current, result);
        }

        public static bool ClosureOfNeeds(List<Teacher> teachers, Dictionary<Lessons, int> personPerLesson)
        {
            // Словарь для подсчета количества преподавателей по каждому предмету
            Dictionary<Lessons, int> coveredLessons = new Dictionary<Lessons, int>();

            // Перебираем всех учителей и их предметы
            foreach (var teacher in teachers)
            {
                foreach (var lesson in teacher.Subjects)
                {
                    if (coveredLessons.ContainsKey(lesson))
                        coveredLessons[lesson]++;
                    else
                        coveredLessons[lesson] = 1;
                }
            }

            // Проверяем, все ли предметы покрыты в нужном количестве
            foreach (var requirement in personPerLesson)
            {
                Lessons lesson = requirement.Key;
                int requiredCount = requirement.Value;

                if (!coveredLessons.TryGetValue(lesson, out int actualCount) || actualCount < requiredCount)
                    return false;  // Предмет не покрыт или преподавателей недостаточно
            }

            return true;
        }
    }
}
