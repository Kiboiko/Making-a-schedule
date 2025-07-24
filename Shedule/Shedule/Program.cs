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

//using System;
//using System.Collections.Generic;
//namespace Shedule
//{
//    class Program
//    {
//        static void Main()
//        {
//            var stud1 = new Student("Veronica", "18:00", "20:00", 1);
//            var stud2 = new Student("Roman", "16:00", "18:00", 3);
//            var stud3 = new Student("Nikita", "17:00", "20:00", 2);
//            var stud4 = new Student("Arthur", "14:00", "17:00", 2);
//            var stud5 = new Student("stud5", "14:00", "17:00", 2);
//            var stud6 = new Student("stud6", "14:00", "17:00", 2);
//            var stud7 = new Student("stud7", "12:00", "17:00", 2);

//            var Stepan = new Teacher("Stepan", "14:00", "20:00",
//                new List<int> { 1, 2, 3 }, 1);

//            var Kirill = new Teacher("Kirill", "15:00", "19:00",
//                new List<int> { 1, 3 }, 1);

//            var Alexander = new Teacher("Alexander", "12:00", "21:00",
//                new List<int> { 1, 2, 3 }, 10);


//            var teacherCombinations = mainMethod.GetTeacherComboForTheDay(
//                new List<Student> { stud1,stud2, stud3, stud4, stud5, stud6, stud7},
//                new List<Teacher> { Stepan,Kirill,Alexander}
//            );

//            Console.WriteLine("\n=== Результат подбора комбинаций на день ===");
//            if (teacherCombinations.Count == 0)
//            {
//                Console.WriteLine("Нет подходящих комбинаций преподавателей!");
//            }
//            else
//            {
//                foreach (var combo in teacherCombinations)
//                {
//                    /*Console.WriteLine($"Комбинация: {string.Join(", ", combo.Select(t => t.Name))}");
//                    Console.WriteLine($"Сумма приоритетов: {combo.Sum(t => t.Priority)}");
//                    Console.WriteLine($"Предметы: {string.Join(", ", combo.SelectMany(t => t.Subjects).Distinct())}");*/

//                    //bool worksAllDay = mainMethod.CheckTeachersComboForTheDay(students, combo);
//                    //Console.WriteLine($"Работает весь день: {(worksAllDay ? "Да" : "Нет")}\n");
//                    Console.WriteLine(String.Join(' ', combo.Select(x => x.Name)));
//                }
//            }

//            Console.ReadLine();
//        }
//    }
//}
using System;
using System.IO;

namespace Shedule
{
    class Program
    {
        static void Main()
        {
            try
            {
                string projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                string credentialsPath = Path.Combine(projectDir, "credentials.json");

                if (!File.Exists(credentialsPath))
                {
                    Console.WriteLine($"Поместите файл 'credentials.json' в:\n{projectDir}");
                    Console.ReadKey();
                    return;
                }

                string spreadsheetId = "1_atz0H3GEjjGE6nSRuzgZklAqqZOSe2Vu-w26N7bRoc";
                var loader = new GoogleSheetsDataLoader(credentialsPath, spreadsheetId);

                // Загрузка данных
                var (teachers, students) = loader.LoadData();
                Console.WriteLine($"Загружено: {teachers.Count} преподавателей, {students.Count} студентов");

                // Генерация расписания
                var combinations = mainMethod.GetTeacherComboForTheDay(students, teachers);
                if (combinations.Count == 0)
                {
                    Console.WriteLine("Нет подходящих комбинаций!");
                    return;
                }

                // Экспорт с обработкой ошибок диапазона
                try
                {
                    var scheduleMatrix = mainMethod.GenerateTeacherScheduleMatrix(students, teachers, combinations);
                    loader.ExportScheduleToGoogleSheets(scheduleMatrix, combinations);
                    Console.WriteLine("Данные успешно экспортированы!");
                }
                catch (Google.GoogleApiException ex) when (ex.Message.Contains("does not match value's range"))
                {
                    Console.WriteLine("Ошибка формата данных при экспорте. Проверьте размерность матрицы расписания.");
                    Console.WriteLine($"Техническая информация: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nНажмите любую клавишу...");
                Console.ReadKey();
            }
        }
    }
}