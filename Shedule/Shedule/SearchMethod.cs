using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /*public static bool CheckComboTeacher(List<Teacher> teachers, List<Student> students)
        {
            TimeOnly minStudTime = students.Select(x=> x.StartOfStudyingTime).ToList().Min();
            TimeOnly currentTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(minStudTime.Hour));
            

        }*/

        
    }
}