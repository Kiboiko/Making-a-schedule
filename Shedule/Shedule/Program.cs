//using System;
//using System.Collections.Generic;
//using System.Security.Cryptography.X509Certificates;

//namespace Shedule
//{
//    class Program
//    {
//        static void Main()
//        {
//            var Stepan = new Teacher("Stepan", "14:00", "20:00",
//                new List<Lessons> { Lessons.Math, Lessons.Informatic }, 1);
//            Console.WriteLine(Stepan.ToString());

//            var Kirill = new Teacher("Kirill", "15:00", "19:00",
//                new List<Lessons> { Lessons.Math, Lessons.Informatic }, 1);
//            Console.WriteLine(Kirill.ToString());
//            var Alexander = new Teacher("Alexander", "12:00", "21:00",
//                new List<Lessons> { Lessons.Math, Lessons.Informatic, Lessons.Physic }, 10);
//            Console.WriteLine(Alexander.ToString());

//            var stud1 = new Student("Veronica", "18:00", "20:00", Lessons.Math);
//            var stud2 = new Student("Roman", "16:00", "18:00", Lessons.Informatic);
//            var stud3 = new Student("Nikita", "17:00", "20:00", Lessons.Math);
//            var stud4 = new Student("Arthur", "14:00", "17:00", Lessons.Physic);
//            Console.WriteLine($"{stud1.ToString()}\n" +
//                $"{stud2.ToString()}\n" +
//                $"{stud3.ToString()}\n" +
//                $"{stud4.ToString()}\n");



//            var TeachList = new List<Teacher> { Stepan, Kirill, Alexander };
//            var studList = new List<Student> { stud1, stud2, stud3, stud4 };
//            mainMethod.SearchForTeachers(TeachList, studList);

//            //var parser = new ExcelParser();
//            //var studentsFromExcel = parser.ParseStudents("example.xlsx");

//            //// Вывод студентов из Excel
//            //Console.WriteLine("\nСтуденты из Excel:");
//            //foreach (var student in studentsFromExcel)
//            //{
//            //    Console.WriteLine(student.ToString());
//            //    Console.WriteLine("------------------");
//            //}
//        }



//    }
//}

using System;
using System.Collections.Generic;
namespace Shedule
{
    class Program
    {
        static void Main()
        {
            


<<<<<<< Updated upstream
=======
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
                    /*Console.WriteLine($"Комбинация: {string.Join(", ", combo.Select(t => t.Name))}");
                    Console.WriteLine($"Сумма приоритетов: {combo.Sum(t => t.Priority)}");
                    Console.WriteLine($"Предметы: {string.Join(", ", combo.SelectMany(t => t.Subjects).Distinct())}");*/

                    //bool worksAllDay = mainMethod.CheckTeachersComboForTheDay(students, combo);
                    //Console.WriteLine($"Работает весь день: {(worksAllDay ? "Да" : "Нет")}\n");
                    Console.WriteLine(String.Join(' ', combo.Select(x => x.Name)));
                }
            }

            Console.ReadLine();
>>>>>>> Stashed changes
        }
    }
}
