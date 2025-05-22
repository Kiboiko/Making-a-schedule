using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;

namespace Shedule
{
    public class ExcelParser
    {
        public List<Student> ParseStudents(string filePath)
        {
            var students = new List<Student>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {Path.GetFullPath(filePath)}");
                return students;
            }

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Первый лист

                    // Чтение данных учеников с 6-й строки
                    for (int row = 6; row <= worksheet.LastRowUsed().RowNumber(); row++)
                    {
                        var nameCell = worksheet.Cell(row, 2);    // Колонка B (имя)
                        var subjectCell = worksheet.Cell(row, 3); // Колонка C (предмет)
                        var timeStartCell = worksheet.Cell(row, 4); // Колонка D (время от)
                        var timeEndCell = worksheet.Cell(row, 5);   // Колонка E (время до)

                        // Пропуск пустых строк
                        if (nameCell.IsEmpty() || subjectCell.IsEmpty()) continue;

                        // Парсинг данных
                        string name = nameCell.Value.ToString();
                        string subjectStr = subjectCell.Value.ToString();
                        string timeStart = timeStartCell.Value.ToString();
                        string timeEnd = timeEndCell.Value.ToString();

                        // Преобразование предмета в enum
                        try
                        {
                            var subject = subjectStr.ParseFromDescription<Lessons>();
                            students.Add(new Student(name, timeStart, timeEnd, subject));
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Ошибка парсинга: {ex.Message} в строке {row}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return students;
        }
    }
}