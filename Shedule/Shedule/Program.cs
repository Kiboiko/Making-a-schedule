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
            /*var loader = new ExcelDataLoader();
            var (teachers, students) = loader.LoadData("../../../example.xlsx");

            // Проверка загруженных данных
            Console.WriteLine("=== Преподаватели ===");
            teachers.ForEach(t => Console.WriteLine(t));


            Console.WriteLine("\n=== Студенты ===");
            // students.ForEach(s => Console.WriteLine(s));

            // Вызов метода подбора
            Console.WriteLine("\n=== Результат ===");
            var MinT = mainMethod.GetMinTeachers(teachers, students);
            foreach (var t in MinT)
            {
                Console.WriteLine(String.Join(' ', t.Select(x => x.Name)));
            }

            Console.ReadLine();*/

            /*TimeOnly curr = TimeOnly.FromTimeSpan(TimeSpan.FromHours(10));
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Итерация {i + 1}: {curr.ToLongTimeString()}");
                curr = curr.AddMinutes(1);
            }*/

            var stud1 = new Student("Veronica", "18:00", "20:00", Lessons.Math);
            var stud2 = new Student("Roman", "16:00", "18:00", Lessons.Informatic);
            var stud3 = new Student("Nikita", "17:00", "20:00", Lessons.Physic);
            var stud4 = new Student("Arthur", "14:00", "17:00", Lessons.Physic);
            var stud5 = new Student("stud5", "14:00", "17:00", Lessons.Physic);
            var stud6 = new Student("stud6", "14:00", "17:00", Lessons.Physic);
            var stud7 = new Student("stud7", "12:00", "17:00", Lessons.Physic);

            var Stepan = new Teacher("Stepan", "14:00", "20:00",
                new List<Lessons> { Lessons.Math, Lessons.Informatic, Lessons.Physic }, 1);

            var Kirill = new Teacher("Kirill", "15:00", "19:00",
                new List<Lessons> { Lessons.Math, Lessons.Informatic }, 1);

            var Alexander = new Teacher("Alexander", "12:00", "21:00",
                new List<Lessons> { Lessons.Math, Lessons.Informatic, Lessons.Physic }, 10);

            var studList = new List<Student> { stud1, stud2, stud3, stud4, stud5, stud6, stud7 };
            var TeachList = new List<Teacher> { Alexander, Stepan, Kirill };
            foreach (var combo in mainMethod.GetTeacherComboForTheDay(students: studList, teachers: TeachList))
            {
                Console.WriteLine(String.Join(' ', combo.Select(x=> x.Name)));
            }
        }
    }
}
