using System;
using System.Collections.Generic;
using System.Linq;

namespace Shedule
{
    class Program
    {
        static void Main()
        {
            var loader = new ExcelDataLoader();
            var (teachers, students) = loader.LoadData("../../../example.xlsx");

            Console.WriteLine("=== Преподаватели ===");
            teachers.ForEach(t => Console.WriteLine(t));

            var teacherCombinations = mainMethod.GetTeacherComboForTheDay(students, teachers);

            Console.WriteLine("\n=== Результат подбора комбинаций на день ===");
            if (teacherCombinations.Count == 0)
            {
                Console.WriteLine("Нет подходящих комбинаций преподавателей!");
            }
            else
            {
                foreach (var combo in teacherCombinations)
                {
                    Console.WriteLine($"Комбинация: {string.Join(", ", combo.Select(t => t.Name))}");
                    Console.WriteLine($"Сумма приоритетов: {combo.Sum(t => t.Priority)}");
                    Console.WriteLine($"Предметы: {string.Join(", ", combo.SelectMany(t => t.Subjects).Distinct())}");

                    //bool worksAllDay = mainMethod.CheckTeachersComboForTheDay(students, combo);
                    //Console.WriteLine($"Работает весь день: {(worksAllDay ? "Да" : "Нет")}\n");
                }
            }

            Console.ReadLine();
        }
    }
}