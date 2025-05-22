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

            int minCountOfTeachers = 0;
            List<Teacher> minCountTeachers = new List<Teacher>();
            var uniqListTeacher = HelperMethods.GetAllTeacherCombinations(teachers);
            foreach (var Combination in uniqListTeacher)
            {
                if (HelperMethods.ClosureOfNeeds(Combination, PersonPerLesson))
                    Console.WriteLine(String.Join(' ', Combination.Select(x => x.Name)));
            }
        }
    
        
    
    
    }
}
