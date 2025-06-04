using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

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
                    var worksheet = workbook.Worksheet(3);// ВЫБОР ЛИСТА С КОТОРЫМ РАБОТАЕМ!!!!!!!

                    foreach (var row in worksheet.RowsUsed())
                    {
                        if (row.RowNumber() == 1) continue; // Пропуск заголовка

                        var typeCell = row.Cell(1).Value.ToString().Trim().ToLower();
                        if (string.IsNullOrEmpty(typeCell)) typeCell = "ученик";

                        if (typeCell == "препод")
                        {
                            var teacher = ParseTeacher(row);
                            if (teacher != null) teachers.Add(teacher);
                        }
                        else
                        {
                            var student = ParseStudent(row);
                            if (student != null) students.Add(student);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return (teachers, students);
        }

        private Teacher ParseTeacher(IXLRow row)
        {
            try
            {
                // Проверка приоритета
                if (!int.TryParse(row.Cell(6).Value.ToString(), out int priority))
                {
                    Console.WriteLine($"Ошибка: неверный приоритет в строке {row.RowNumber()}");
                    return null;
                }

                return new Teacher(
                    name: row.Cell(2).Value.ToString().Trim(),
                    startOfStudyTime: row.Cell(4).Value.ToString(),
                    endOfStudyTime: row.Cell(5).Value.ToString(),
                    _lessons: ParseSubjects(row.Cell(3).Value.ToString()),
                    priority: priority
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга преподавателя: {ex.Message}");
                return null;
            }
        }

        private List<Lessons> ParseSubjects(string input)
        {
            var subjects = new List<Lessons>();
            foreach (var subject in input.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    var trimmedSubject = subject.Trim();
                    subjects.Add(trimmedSubject.ParseFromDescription<Lessons>());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка парсинга предмета '{subject}': {ex.Message}");
                }
            }
            return subjects;
        }

        private Student ParseStudent(IXLRow row)
        {
            try
            {
                return new Student(
                    name: row.Cell(2).Value.ToString().Trim(),
                    startOfStudyTime: row.Cell(7).Value.ToString(),
                    endOfStudyTime: row.Cell(8).Value.ToString(),
                    subject: row.Cell(3).Value.ToString().ParseFromDescription<Lessons>()
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга студента: {ex.Message}");
                return null;
            }
        }
    }
}