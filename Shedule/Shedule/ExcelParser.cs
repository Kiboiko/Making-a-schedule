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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shedule
{
    public class ExcelDataLoader
    {
        public (List<Teacher> teachers, List<Student> students) LoadData(string filePath)
        {
            var teachers = new List<Teacher>();
            var students = new List<Student>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {filePath}");
                return (teachers, students);
            }

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    // Загрузка справочника предметов
                    var subjectMap = LoadSubjectMap(workbook);

                    // Лист преподавателей
                    var teacherSheet = workbook.Worksheet("преподы");
                    if (teacherSheet == null) throw new Exception("Лист 'преподы' не найден");

                    foreach (var row in teacherSheet.RowsUsed().Skip(1))
                    {
                        if (row.IsEmpty()) continue;
                        var teacher = ParseTeacherRow(row, subjectMap);
                        if (teacher != null) teachers.Add(teacher);
                    }

                    // Лист студентов
                    var studentSheet = workbook.Worksheet("ученики");
                    if (studentSheet == null) throw new Exception("Лист 'ученики' не найден");

                    foreach (var row in studentSheet.RowsUsed().Skip(1))
                    {
                        if (row.IsEmpty()) continue;
                        var student = ParseStudentRow(row, subjectMap);
                        if (student != null) students.Add(student);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            }

            return (teachers, students);
        }

        private Dictionary<string, Lessons> LoadSubjectMap(XLWorkbook workbook)
        {
            var map = new Dictionary<string, Lessons>();

            var sheet = workbook.Worksheet("квалификации");
            if (sheet == null) return map;

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                var subjectName = row.Cell(1).Value.ToString().Trim();
                if (int.TryParse(row.Cell(2).Value.ToString(), out int id))
                {
                    try
                    {
                        var lesson = subjectName.ParseFromDescription<Lessons>();
                        map[id.ToString()] = lesson;
                    }
                    catch
                    {
                        // Игнорируем невалидные предметы
                    }
                }
            }
            return map;
        }

        private Teacher ParseTeacherRow(IXLRow row, Dictionary<string, Lessons> subjectMap)
        {
            try
            {
                // Столбцы: A-пустой, B-имя, C-предметы, D-начало, E-конец, F-приоритет
                string name = row.Cell(2).Value.ToString().Trim();
                string subjectsInput = row.Cell(3).Value.ToString().Trim();
                string startTime = NormalizeTime(row.Cell(4).Value.ToString());
                string endTime = NormalizeTime(row.Cell(5).Value.ToString());
                int priority = int.TryParse(row.Cell(6).Value.ToString(), out int p) ? p : 1;

                // Парсинг предметов с поддержкой . и , как разделителей
                var subjectIds = subjectsInput.Split(new[] { ',', '.', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(id => id.Trim())
                                              .ToList();

                var lessons = new List<Lessons>();
                foreach (var id in subjectIds)
                {
                    if (subjectMap.TryGetValue(id, out var lesson))
                    {
                        lessons.Add(lesson);
                    }
                }

                return new Teacher(name, startTime, endTime, lessons, priority);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга преподавателя: {ex.Message}");
                return null;
            }
        }

        private Student ParseStudentRow(IXLRow row, Dictionary<string, Lessons> subjectMap)
        {
            try
            {
                // Столбцы: A-пустой, B-имя, C-предмет, D-начало, E-конец
                string name = row.Cell(2).Value.ToString().Trim();
                string subjectId = row.Cell(3).Value.ToString().Trim();
                string startTime = NormalizeTime(row.Cell(4).Value.ToString());
                string endTime = NormalizeTime(row.Cell(5).Value.ToString());

                if (!subjectMap.TryGetValue(subjectId, out var lesson))
                {
                    throw new ArgumentException($"Неизвестный ID предмета: {subjectId}");
                }

                return new Student(name, startTime, endTime, lesson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга студента: {ex.Message}");
                return null;
            }
        }

        private string NormalizeTime(string timeStr)
        {
            // Упрощаем "10:00:00" до "10:00"
            if (timeStr.Length > 5 && timeStr.Contains(':'))
            {
                var parts = timeStr.Split(':');
                if (parts.Length >= 2)
                {
                    return $"{parts[0]}:{parts[1]}";
                }
            }
            return timeStr;
        }
    }
}