//using ClosedXML.Excel;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;

//namespace Shedule
//{
//    public class ExcelDataLoader
//    {
//        public (List<Teacher> teachers, List<Student> students) LoadData(string filePath)
//        {
//            var teachers = new List<Teacher>();
//            var students = new List<Student>();

//            if (!File.Exists(filePath))
//            {
//                Console.WriteLine($"Файл не найден: {filePath}");
//                return (teachers, students);
//            }

//            //    try
//            //    {
//            //        using (var workbook = new XLWorkbook(filePath))
//            //        {
//            //            var worksheet = workbook.Worksheet(2);

//            //            foreach (var row in worksheet.RowsUsed())
//            //            {
//            //                if (row.RowNumber() == 1) continue; // Пропуск заголовка

//            //                var typeCell = row.Cell(1).Value.ToString().Trim().ToLower();
//            //                if (string.IsNullOrEmpty(typeCell)) typeCell = "ученик";

//            //                if (typeCell == "препод")
//            //                {
//            //                    var teacher = ParseTeacher(row);
//            //                    if (teacher != null) teachers.Add(teacher);
//            //                }
//            //                else
//            //                {
//            //                    var student = ParseStudent(row);
//            //                    if (student != null) students.Add(student);
//            //                }
//            //            }
//            //        }
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        Console.WriteLine($"Ошибка: {ex.Message}");
//            //    }

//            //    return (teachers, students);
//            //}
//            // ПРОБНОЕ ИЗМЕНЕНИЕ ПАРСИНГА 
//            try
//            {
//                using (var workbook = new XLWorkbook(filePath))
//                {
//                    var worksheetprep = workbook.Worksheet(1);
//                    var worksheetstud = workbook.Worksheet(2);
//                    var worksheetkval = workbook.Worksheet(3);

//                    foreach (var row in worksheetprep.RowsUsed())
//                    {
//                        if (row.RowNumber() == 1) continue; // Пропуск заголовка

//                        var typeCell = row.Cell(1).Value.ToString().Trim().ToLower();
//                        //if (string.IsNullOrEmpty(typeCell)) typeCell = "ученик";

//                        if (typeCell == "препод")
//                        {
//                            var teacher = ParseTeacher(row);
//                            if (teacher != null) teachers.Add(teacher);
//                        }
//                        //else
//                        //{
//                        //    var student = ParseStudent(row);
//                        //    if (student != null) students.Add(student);
//                        //}
//                    }

//                    foreach (var row in worksheetstud.RowsUsed())
//                    {
//                        if (row.RowNumber() == 1) continue;
//                        var typeCell = row.Cell(1).Value.ToString().Trim().ToLower();
//                        if (typeCell == "ученик")
//                        {
//                            var student = ParseStudent(row);
//                            if (student != null) student.Add(student);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Ошибка: {ex.Message}");
//            }

//            return (teachers, students);
//        }
//        //ПРОБНОЕ ИЗМЕНЕНИЕ ПАРСИНГА

//        private Teacher ParseTeacher(IXLRow row)
//        {
//            try
//            {
//                // Проверка приоритета
//                if (!int.TryParse(row.Cell(6).Value.ToString(), out int priority))
//                {
//                    Console.WriteLine($"Ошибка: неверный приоритет в строке {row.RowNumber()}");
//                    return null;
//                }

//                return new Teacher(
//                    name: row.Cell(2).Value.ToString().Trim(),
//                    startOfStudyTime: row.Cell(4).Value.ToString(),
//                    endOfStudyTime: row.Cell(5).Value.ToString(),
//                    _lessons: ParseSubjects(row.Cell(3).Value.ToString()),
//                    priority: priority
//                );
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Ошибка парсинга преподавателя: {ex.Message}");
//                return null;
//            }
//        }

//        private List<Lessons> ParseSubjects(string input)
//        {
//            var subjects = new List<Lessons>();
//            foreach (var subject in input.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
//            {
//                try
//                {
//                    var trimmedSubject = subject.Trim();
//                    subjects.Add(trimmedSubject.ParseFromDescription<Lessons>());
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Ошибка парсинга предмета '{subject}': {ex.Message}");
//                }
//            }
//            return subjects;
//        }

//        private Student ParseStudent(IXLRow row)
//        {
//            try
//            {
//                return new Student(
//                    name: row.Cell(2).Value.ToString().Trim(),
//                    startOfStudyTime: row.Cell(4).Value.ToString(),
//                    endOfStudyTime: row.Cell(5).Value.ToString(),
//                    subject: row.Cell(3).Value.ToString().ParseFromDescription<Lessons>()
//                );
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Ошибка парсинга студента: {ex.Message}");
//                return null;
//            }
//        }
//    }
//}

using ClosedXML.Excel;
using Shedule;

public class ExcelDataLoader
{
    public (List<Teacher> teachers, List<Student> students) LoadData(string filePath)
    {
        var teachers = new List<Teacher>();
        var students = new List<Student>();

        using (var workbook = new XLWorkbook(filePath))
        {
            // Лист преподавателей (индекс 1)
            var teacherSheet = workbook.Worksheet(1);
            foreach (var row in teacherSheet.RowsUsed().Skip(1)) // Пропуск заголовка
            {
                if (row.IsEmpty()) continue;
                var teacher = ParseTeacher(row);
                if (teacher != null) teachers.Add(teacher);
            }

            // Лист студентов (индекс 2)
            var studentSheet = workbook.Worksheet(2);
            foreach (var row in studentSheet.RowsUsed().Skip(1)) // Пропуск заголовка
            {
                if (row.IsEmpty()) continue;
                var student = ParseStudent(row);
                if (student != null) students.Add(student);
            }
        }
        return (teachers, students);
    }

    private Teacher ParseTeacher(IXLRow row)
    {
        try
        {
            return new Teacher(
                name: row.Cell(1).Value.ToString().Trim(),
                startOfStudyTime: row.Cell(3).Value.ToString(),
                endOfStudyTime: row.Cell(4).Value.ToString(),
                _lessons: ParseSubjectIds(row.Cell(2).Value.ToString()),
                priority: int.Parse(row.Cell(5).Value.ToString())
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка парсинга преподавателя: {ex.Message}");
            return null;
        }
    }

    private List<Lessons> ParseSubjectIds(string input)
    {
        return input.Split(',')
                   .Select(id => (Lessons)int.Parse(id.Trim()))
                   .ToList();
    }

    private Student ParseStudent(IXLRow row)
    {
        try
        {
            return new Student(
                name: row.Cell(1).Value.ToString().Trim(),
                startOfStudyTime: row.Cell(3).Value.ToString(),
                endOfStudyTime: row.Cell(4).Value.ToString(),
                subject: (Lessons)int.Parse(row.Cell(2).Value.ToString())
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка парсинга студента: {ex.Message}");
            return null;
        }
    }
}