using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml.Office;

namespace Shedule
{
    public class mainMethod
    {

        public static List<List<Teacher>> GetMinTeachers(List<Teacher> teachers, List<Student> students)
        {
            List<List<Teacher>> res = new List<List<Teacher>>();
            List<List<Teacher>> uniqTeachers = HelperMethods.GetAllTeacherCombinations(teachers);
            int minCount = int.MaxValue;
            foreach(var combo in uniqTeachers)
            {
                if (School.CheckTeacherStudentAllocation(combo, students) && combo.Count != 0)
                {
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

        public static List<Teacher> GetActiveTeachersAtMinute(List<Teacher> teachers, TimeOnly time)
        {
            List<Teacher> res = new List<Teacher>();
            foreach (var teacher in teachers)
            {
                if ((time >= teacher.StartOfStudyingTime) & (time <= teacher.EndOfStudyingTime))
                {
                    res.Add(teacher);
                }
            }
            return res;

        }

        public static List<Student> GetActiveStudentsAtMinute(List<Student> students, TimeOnly time)
        {
            List<Student> res = new List<Student>();
            foreach (var student in students)
            {
                if ((time >= student.StartOfStudyingTime) & (time <= student.EndOfStudyingTime))
                {
                    res.Add(student);
                }
            }
            return res;

        }

        public static bool CheckTeachersComboPerMinute(List<Student> students, List<Teacher> teachers, TimeOnly time)
        {
            return School.CheckTeacherStudentAllocation(
                GetActiveTeachersAtMinute(teachers,time), 
                GetActiveStudentsAtMinute(students,time)
                );
        }


        public static bool CheckTeachersComboForTheDay(List<Student> students, List<Teacher> teachers)
        {
            TimeOnly startStudTime = students.Select(x => x.StartOfStudyingTime).ToList().Min();
            TimeOnly currentTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(9));
            bool t = true;
            for (int i = 0; i < 660; i++)
            {
                currentTime = currentTime.AddMinutes(1);
                if (!CheckTeachersComboPerMinute(students, teachers, currentTime))
                {
                    t = false;
                    break;
                }
            }
            return t;
        }


        public static List<List<Teacher>> GetTeacherComboForTheDay(List<Student> students, List<Teacher> teachers)
        {
            List<List<Teacher>> uniqTeachers = HelperMethods.GetAllTeacherCombinations(teachers).OrderBy(x => x.Count).ToList();
            List<List<Teacher>> res = new List<List<Teacher>>();
            
            foreach (var combo in uniqTeachers)
            {
                if (CheckTeachersComboForTheDay(students, combo) && CheckForEntryinterruption(res,combo))
                {
                    res.Add(combo);
                }
            }
            return res.OrderBy(x => x.Select(y => y.Priority).Sum()).ToList();
        }

        public static bool CheckForEntryinterruption(List<List<Teacher>> res, List<Teacher> combo)
        {
            if (res.Count == 0)
                return true;
            var t = true;
            foreach (var item in res)
            {
                if (FindingAnOccurrenceOfaCombination(combo, item))
                {
                    t = false;
                    break;
                }
            }
            return t;
        }

        public static bool FindingAnOccurrenceOfaCombination(List<Teacher> item, List<Teacher> combo)
        {
            int c = 0;
            foreach (var teacher in combo)
            {
                if (item.Select(x=>x.Name).ToList().Contains(teacher.Name))
                {
                    c++;
                }
            }
            return ((c == combo.Count()));
        }


        public static void ShowWorkingTimeTeachers(List<Teacher> teachers, List<Student> students)
        {
            TimeOnly startStudTime = students.Select(x => x.StartOfStudyingTime).ToList().Min();
            TimeOnly currentTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(9));
            for (int i = 0; i < 660; i++)
            {
                currentTime = currentTime.AddMinutes(1);
                Console.WriteLine($"{currentTime.ToString()},{String.Join(' ',
                    mainMethod.GetMinTeachers(teachers, students)[0].Select(x => x.Name))}");
            }
        }




    }
}