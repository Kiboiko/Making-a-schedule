using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public class HelperMethods
    {
        public static List<List<Teacher>> GetAllTeacherCombinations(List<Teacher> teachers)
        {
            return GenerateCombinations(teachers).ToList();
        }

        private static IEnumerable<List<Teacher>> GenerateCombinations(List<Teacher> teachers)
        {
            int totalCombinations = 1 << teachers.Count;

            for (int mask = 0; mask < totalCombinations; mask++)
            {
                var combination = new List<Teacher>();
                for (int i = 0; i < teachers.Count; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        combination.Add(teachers[i]);
                    }
                }
                yield return combination;
            }
        }

        /*public static bool ClosureOfNeeds(List<Teacher> teachers, Dictionary<Lessons, int> personPerLesson)
        {
            // Словарь для подсчета преподавателей по предметам (с учетом их загрузки)
            Dictionary<Lessons, int> availableTeachersPerLesson = new Dictionary<Lessons, int>();

            foreach (var teacher in teachers)
            {
                foreach (var lesson in teacher.Subjects)
                {
                    // Если у преподавателя есть место (<4 учеников), учитываем его
                    if (teacher.CountStudents < 4)
                    {
                        if (availableTeachersPerLesson.ContainsKey(lesson))
                            availableTeachersPerLesson[lesson]++;
                        else
                            availableTeachersPerLesson[lesson] = 1;
                    }
                }
            }

            // Проверяем, хватает ли преподавателей с учетом их загрузки
            foreach (var requirement in personPerLesson)
            {
                Lessons lesson = requirement.Key;
                int requiredCount = requirement.Value;

                if (!availableTeachersPerLesson.TryGetValue(lesson, out int actualCount) || actualCount < requiredCount)
                    return false;
            }

            return true;
        }*/

        public static bool ClosureOfNeeds(List<Teacher> teachers, Dictionary<Lessons, int> personPerLesson, List<Student> students)
        {
            // Словарь для подсчета доступных преподавателей по предметам (с учетом времени и нагрузки)
            Dictionary<Lessons, int> availableTeachersPerLesson = new Dictionary<Lessons, int>();

            foreach (var teacher in teachers)
            {
                foreach (var lesson in teacher.Subjects)
                {
                    // Проверяем, есть ли у преподавателя свободные места (<4 учеников)
                    if (teacher.CountStudents >= 4)
                        continue;

                    // Проверяем, есть ли ученики по этому предмету
                    if (!personPerLesson.ContainsKey(lesson))
                        continue;

                    // Проверяем, есть ли хотя бы один ученик, чье время пересекается с временем учителя
                    bool hasTimeOverlap = students.Any(s =>
                        s.Subject == lesson &&
                        TimeOverlaps(s.StartOfStudyingTime, s.EndOfStudyingTime,
                                     teacher.StartOfStudyingTime, teacher.EndOfStudyingTime));

                    if (hasTimeOverlap)
                    {
                        if (availableTeachersPerLesson.ContainsKey(lesson))
                            availableTeachersPerLesson[lesson]++;
                        else
                            availableTeachersPerLesson[lesson] = 1;
                    }
                }
            }

            // Проверяем, хватает ли преподавателей с учетом нагрузки и времени
            foreach (var requirement in personPerLesson)
            {
                Lessons lesson = requirement.Key;
                int requiredCount = requirement.Value;

                if (!availableTeachersPerLesson.TryGetValue(lesson, out int actualCount) ||
                    actualCount * 4 < requiredCount)  // Учитываем, что 1 учитель = максимум 4 ученика
                    return false;
            }

            return true;
        }

        // Проверяет, пересекаются ли два временных интервала
        private static bool TimeOverlaps(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
        {
            return start1 < end2 && start2 < end1;
        }
    }
}
