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
