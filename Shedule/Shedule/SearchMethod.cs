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
            foreach (Student student in students)
            {
                if (!PersonPerLesson.ContainsKey(student.Subject))
                {
                    PersonPerLesson[student.Subject] = 1;
                }
                else
                {
                    PersonPerLesson[student.Subject] += 1;
                }
            }
            foreach (Lessons lesson in PersonPerLesson.Keys)
            {
                Console.WriteLine($"{lesson} - {PersonPerLesson[lesson]}");
            }

            int minCountOfTeachers = int.MaxValue;
            List<List<Teacher>> minCountTeachersList = new List<List<Teacher>>();
            var uniqListTeacher = HelperMethods.GetAllTeacherCombinations(teachers);
            foreach (var Combination in uniqListTeacher)
            {
                if (HelperMethods.ClosureOfNeeds(Combination, PersonPerLesson) &&
                    Combination.Count <= minCountOfTeachers)
                {
                    minCountOfTeachers = Combination.Count;
                    if (Combination.Count == minCountOfTeachers)
                        minCountTeachersList.Add(Combination);
                    else
                    {
                        minCountTeachersList.Clear();
                        minCountTeachersList.Add(Combination);
                    }
                }
            }
            foreach (var comb in minCountTeachersList)
            {
                Console.WriteLine(String.Join(' ', comb.Select(x => x.Name)));
            }
        }
    }
}
