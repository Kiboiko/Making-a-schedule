using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public class mainMethod
    {
        /*public static void SearchForTeachers(List<Teacher> teachers, List<Student> students)
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
        }*/

        public static List<List<Teacher>> GetMinTeachers(List<Teacher> teachers, List<Student> students)
        {
            List<List<Teacher>> res = new List<List<Teacher>>();
            List<List<Teacher>> uniqTeachers = HelperMethods.GetAllTeacherCombinations(teachers);
            /*foreach(var t in uniqTeachers)
            {
                Console.WriteLine(string.Join(' ', t.Select(x => x.Name)));
            }*/
            int minCount = int.MaxValue;
            foreach(var combo in uniqTeachers)
            {
                if (School.CheckTeacherStudentAllocation(combo, students) && combo.Count != 0)
                {
                    //Console.WriteLine(string.Join(' ',combo.Select(x=>x.Name)));
                    if (combo.Count < minCount)
                    {
                        res.Clear();
                        res.Add(combo);
                        minCount = combo.Count;
                    } else if (combo.Count == minCount)
                    {
                        res.Add(combo);
                    } 
                }
            }
            return res;
        }
    }
}