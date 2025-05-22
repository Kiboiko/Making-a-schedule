using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public class mainMethod
    {
        public static void SearchForTeachers(List<Teacher> teachers, List<Student> students)
        {
            List<Teacher> Ans = new List<Teacher>();
            Dictionary<Lessons, int> PersonPerLesson = new Dictionary<Lessons, int>();

            // Считаем количество учеников по предметам
            foreach (Student student in students)
            {
                if (!PersonPerLesson.ContainsKey(student.Subject))
                    PersonPerLesson[student.Subject] = 1;
                else
                    PersonPerLesson[student.Subject] += 1;
            }

            int minCountOfTeachers = int.MaxValue;
            List<List<Teacher>> minCountTeachersList = new List<List<Teacher>>();

            // Перебираем все комбинации учителей
            var uniqListTeacher = HelperMethods.GetAllTeacherCombinations(teachers);
            foreach (var Combination in uniqListTeacher)
            {
                if (HelperMethods.ClosureOfNeeds(Combination, PersonPerLesson, students) &&
                    Combination.Count <= minCountOfTeachers)
                {
                    if (Combination.Count < minCountOfTeachers)
                    {
                        minCountOfTeachers = Combination.Count;
                        minCountTeachersList.Clear();
                    }
                    minCountTeachersList.Add(Combination);
                }
            }

            // Выводим результаты
            foreach (var comb in minCountTeachersList)
            {
                Console.WriteLine(String.Join(' ', comb.Select(x => x.Name)));
            }
        }
    }
}