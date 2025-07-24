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


        public static object[,] GenerateTeacherScheduleMatrix(List<Student> students, List<Teacher> teachers, List<List<Teacher>> teacherCombinations)
        {
            TimeOnly startTime = new TimeOnly(9, 0);
            TimeOnly endTime = new TimeOnly(20, 0);
            int totalMinutes = (int)(endTime - startTime).TotalMinutes;
            int timeSlots = (int)Math.Ceiling(totalMinutes / 15.0);

            object[,] matrix = new object[teachers.Count + 1, timeSlots + 1];

            // Заполняем заголовки
            matrix[0, 0] = "Teachers/Time";
            for (int i = 1; i <= timeSlots; i++)
            {
                TimeOnly slotStart = startTime.AddMinutes((i - 1) * 15);
                TimeOnly slotEnd = slotStart.AddMinutes(15);
                matrix[0, i] = $"{slotStart:HH:mm}-{slotEnd:HH:mm}";
            }

            // Заполняем имена преподавателей
            for (int i = 0; i < teachers.Count; i++)
            {
                matrix[i + 1, 0] = teachers[i].Name;
            }

            // Обрабатываем каждый тайм-слот
            for (int slot = 1; slot <= timeSlots; slot++)
            {
                TimeOnly slotStart = startTime.AddMinutes((slot - 1) * 15);
                TimeOnly slotEnd = slotStart.AddMinutes(15);

                // Получаем активных студентов в этом тайм-слоте
                List<Student> activeStudents = students
                    .Where(s => s.StartOfStudyingTime < slotEnd &&
                               s.EndOfStudyingTime >= slotStart)
                    .ToList();

                // Если нет активных студентов - все преподаватели "0"
                if (activeStudents.Count == 0)
                {
                    for (int teacherRow = 1; teacherRow <= teachers.Count; teacherRow++)
                    {
                        matrix[teacherRow, slot] = "0";
                    }
                    continue;
                }

                // Получаем список предметов, которые нужны в этот тайм-слот
                var requiredSubjects = activeStudents
                    .Select(s => s.SubjectId)
                    .Distinct()
                    .ToList();

                // Находим активные комбинации для этого тайм-слота
                List<int> activeCombinationIndices = new List<int>();
                for (int i = 0; i < teacherCombinations.Count; i++)
                {
                    var activeTeachersInCombo = teacherCombinations[i]
                        .Where(t => t.StartOfStudyingTime <= slotStart &&
                                   t.EndOfStudyingTime >= slotEnd)
                        .ToList();

                    if (activeTeachersInCombo.Count > 0 &&
                        School.CheckTeacherStudentAllocation(activeTeachersInCombo, activeStudents))
                    {
                        activeCombinationIndices.Add(i + 1);
                    }
                }

                // Заполняем данные для каждого преподавателя
                for (int teacherRow = 1; teacherRow <= teachers.Count; teacherRow++)
                {
                    Teacher teacher = teachers[teacherRow - 1];

                    // 1. Проверяем, работает ли преподаватель в этот тайм-слот (строгая проверка)
                    bool isTeacherActive = teacher.StartOfStudyingTime <= slotStart &&
                                         teacher.EndOfStudyingTime >= slotEnd;

                    if (!isTeacherActive)
                    {
                        matrix[teacherRow, slot] = "0";
                        continue;
                    }

                    // 2. Проверяем, есть ли студенты, которым нужен этот преподаватель
                    bool isTeacherNeeded = teacher.SubjectsId
                        .Any(subjectId => requiredSubjects.Contains(subjectId));

                    if (!isTeacherNeeded)
                    {
                        matrix[teacherRow, slot] = "0";
                        continue;
                    }

                    // 3. Если преподаватель активен и нужен, проверяем комбинации
                    var relevantCombos = activeCombinationIndices
                        .Where(comboIndex => teacherCombinations[comboIndex - 1]
                            .Any(t => t.Name == teacher.Name))
                        .ToList();

                    matrix[teacherRow, slot] = relevantCombos.Count > 0
                        ? string.Join(",", relevantCombos)
                        : "0";
                }
            }

            return matrix;
        }

        public static void PrintTeacherScheduleMatrix(object[,] matrix, List<List<Teacher>> teacherCombinations)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            // Определяем максимальную ширину для каждого столбца
            int[] columnWidths = new int[cols];
            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    int length = matrix[row, col]?.ToString()?.Length ?? 0;
                    if (length > columnWidths[col])
                    {
                        columnWidths[col] = length;
                    }
                }
                // Минимальная ширина для столбца - 3 символа
                columnWidths[col] = Math.Max(columnWidths[col], 3);
            }

            Console.WriteLine("РАСПИСАНИЕ ПРЕПОДАВАТЕЛЕЙ ПО ТАЙМ-СЛОТАМ");

            for (int col = 0; col < cols; col++)
            {
                string format = $"| {{0,-{columnWidths[col]}}} ";
                Console.Write(format, matrix[0, col]);
            }
            Console.WriteLine("|");


            for (int row = 1; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    string format = $"| {{0,-{columnWidths[col]}}} ";
                    Console.Write(format, matrix[row, col]);
                }
                Console.WriteLine("|");
            }

            Console.WriteLine("\nСПИСОК КОМБИНАЦИЙ ПРЕПОДАВАТЕЛЕЙ:");
            for (int i = 0; i < teacherCombinations.Count; i++)
            {
                string comboTeachers = string.Join(", ", teacherCombinations[i].Select(t => t.Name));
                Console.WriteLine($"[{i + 1}] {comboTeachers}");
            }
        }




    }
}