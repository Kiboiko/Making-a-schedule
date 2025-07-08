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
                /*TimeSpan startTime = TimeSpan.Parse(startTimeStr);
                TimeSpan endTime = TimeSpan.Parse(endTimeStr);
                TimeSpan workingTime = endTime - startTime;
                int maximumAttention = (int)workingTime.TotalMinutes;*/
                int maximumAttention = 15;

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

        public static void ExportScheduleToExcel(object[,] matrix, List<List<Teacher>> teacherCombinations, string originalFilePath)
        {
            if (!File.Exists(originalFilePath))
            {
                throw new FileNotFoundException("Исходный файл не найден", originalFilePath);
            }

            try
            {
                using (var workbook = new XLWorkbook(originalFilePath))
                {
                    // Удаляем старый лист, если он существует
                    RemoveWorksheetIfExists(workbook, "Расписание и комбинации");

                    // Создаем новый лист
                    var worksheet = workbook.Worksheets.Add("Расписание и комбинации");

                    // 1. Заполняем таблицу расписания
                    int startRow = 1;
                    FillScheduleSheet(worksheet, matrix, startRow);

                    // Добавляем границы для всей таблицы расписания
                    int lastScheduleRow = startRow + matrix.GetLength(0) - 1;
                    int lastScheduleCol = matrix.GetLength(1);
                    var scheduleRange = worksheet.Range(startRow, 1, lastScheduleRow, lastScheduleCol);
                    scheduleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    scheduleRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // 2. Добавляем заголовок для комбинаций (2 строки после таблицы)
                    int comboStartRow = lastScheduleRow + 3;
                    worksheet.Cell(comboStartRow, 1).Value = "Список комбинаций преподавателей";
                    worksheet.Cell(comboStartRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(comboStartRow, 1).Style.Font.FontSize = 12;

                    // 3. Заполняем список комбинаций
                    FillCombinationsSheet(worksheet, teacherCombinations, comboStartRow + 1);

                    // Сохраняем изменения
                    workbook.Save();
                    Console.WriteLine($"Данные успешно добавлены в файл: {originalFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в файл: {ex.Message}");
                throw;
            }
        }

        private static void RemoveWorksheetIfExists(XLWorkbook workbook, string sheetName)
        {
            if (workbook.Worksheets.Any(ws => ws.Name == sheetName))
            {
                workbook.Worksheets.Delete(sheetName);
            }
        }

        private static void FillScheduleSheet(IXLWorksheet sheet, object[,] matrix, int startRow)
        {
            // 1. Настраиваем столбец с именами преподавателей
            sheet.Column(1).Width = GetOptimalColumnWidth(matrix, 0) + 2; // +2 для запаса

            // 2. Настраиваем столбцы с тайм-слотами
            for (int col = 1; col < matrix.GetLength(1); col++)
            {
                sheet.Column(col + 1).Width = GetOptimalColumnWidth(matrix, col);
            }

            // 3. Заполняем данные с сохранением всех стилей
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    var cell = sheet.Cell(startRow + row, col + 1);
                    cell.Value = matrix[row, col]?.ToString();

                    // Стилизация ячеек
                    if (row == 0) // Заголовки
                    {
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    else if (matrix[row, col]?.ToString() == "0")
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                        cell.Style.Font.FontColor = XLColor.DarkGray;
                    }
                    else if (col == 0) // Имена преподавателей
                    {
                        cell.Style.Font.Bold = true;
                    }
                    else // Активные слоты
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                    }

                    // Границы
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
            }
        }

        // Метод для расчета оптимальной ширины столбца
        private static double GetOptimalColumnWidth(object[,] matrix, int colIndex)
        {
            double maxWidth = 8.0; // Минимальная ширина по умолчанию

            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                string content = matrix[row, colIndex]?.ToString() ?? "";
                double contentWidth = content.Length * 1.2; // Эмпирический коэффициент

                if (colIndex == 0) // Для столбца с именами
                    contentWidth = content.Length * 1.5;

                if (contentWidth > maxWidth)
                    maxWidth = contentWidth;
            }

            return Math.Min(maxWidth, 50.0); // Ограничиваем максимальную ширину
        }

        private static void FillCombinationsSheet(IXLWorksheet sheet, List<List<Teacher>> teacherCombinations, int startRow)
        {
            // Заголовки таблицы комбинаций
            sheet.Cell(startRow, 1).Value = "№";
            sheet.Cell(startRow, 2).Value = "Состав комбинации";

            var headerRange = sheet.Range(startRow, 1, startRow, 2);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Данные комбинаций
            for (int i = 0; i < teacherCombinations.Count; i++)
            {
                sheet.Cell(startRow + i + 1, 1).Value = i + 1;
                sheet.Cell(startRow + i + 1, 2).Value = string.Join(", ", teacherCombinations[i].Select(t => t.Name));

                // Добавляем границы для каждой строки комбинаций
                var rowRange = sheet.Range(startRow + i + 1, 1, startRow + i + 1, 2);
                rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Настраиваем ширину столбцов
            sheet.Column(1).Width = 5;  // Номер
            sheet.Column(2).Width = 50; // Состав комбинации
        }
    }
}