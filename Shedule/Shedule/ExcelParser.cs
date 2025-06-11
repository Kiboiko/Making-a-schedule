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

        private Dictionary<string, int> LoadSubjectMap(XLWorkbook workbook)
        {
            var map = new Dictionary<string, int>();
            var sheet = workbook.Worksheet("квалификации");
            if (sheet == null) return map;

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                var subjectName = row.Cell(1).Value.ToString().Trim();
                if (int.TryParse(row.Cell(2).Value.ToString(), out int id))
                {
                    map[subjectName] = id;
                }
            }
            return map;
        }

        private Teacher ParseTeacherRow(IXLRow row, Dictionary<string, int> subjectMap)
        {
            try
            {
                // Основные данные
                string name = row.Cell(2).Value.ToString().Trim();
                string subjectsInput = row.Cell(3).Value.ToString().Trim();
                string startTimeStr = row.Cell(4).Value.ToString();
                string endTimeStr = row.Cell(5).Value.ToString();
                int priority = int.TryParse(row.Cell(6).Value.ToString(), out int p) ? p : 1;

                // Расчет MaximumAttention на основе рабочего времени
                TimeSpan startTime = TimeSpan.Parse(startTimeStr);
                TimeSpan endTime = TimeSpan.Parse(endTimeStr);
                TimeSpan workingTime = endTime - startTime;
                int maximumAttention = (int)workingTime.TotalMinutes;

                // Парсинг ID предметов
                var subjectIds = subjectsInput
                    .Split(new[] { ',', '.', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => id.Trim())
                    .Where(id => subjectMap.ContainsValue(int.Parse(id)))
                    .Select(id => int.Parse(id))
                    .ToList();

                return new Teacher(
                    name: name,
                    startOfStudyTime: NormalizeTime(startTimeStr),
                    endOfStudyTime: NormalizeTime(endTimeStr),
                    _lessons: subjectIds,
                    priority: priority,
                    _MaximumAttention: maximumAttention
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга преподавателя (строка {row.RowNumber()}): {ex.Message}");
                return null;
            }
        }

        private Student ParseStudentRow(IXLRow row, Dictionary<string, int> subjectMap)
        {
            try
            {
                // Основные данные
                string name = row.Cell(2).Value.ToString().Trim();
                string subjectId = row.Cell(3).Value.ToString().Trim();
                string startTimeStr = row.Cell(4).Value.ToString();
                string endTimeStr = row.Cell(5).Value.ToString();

                // Потребность во внимании (обязательное поле)
                if (!int.TryParse(row.Cell(6).Value.ToString(), out int needForAttention))
                {
                    throw new ArgumentException($"Не указана потребность во внимании для студента {name}");
                }

                if (!int.TryParse(subjectId, out int subjectIdInt))
                {
                    throw new ArgumentException($"Неверный ID предмета: {subjectId}");
                }

                return new Student(
                    name: name,
                    startOfStudyTime: NormalizeTime(startTimeStr),
                    endOfStudyTime: NormalizeTime(endTimeStr),
                    subjectId: subjectIdInt,
                    _NeedForAttention: needForAttention
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга студента (строка {row.RowNumber()}): {ex.Message}");
                return null;
            }
        }

        private string NormalizeTime(string timeStr)
        {
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