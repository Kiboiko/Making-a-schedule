using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public class School
    {
        public static bool CheckTeacherStudentAllocation(List<Teacher> teachers, List<Student> students)
        {
            var teacherAssignments = new Dictionary<Teacher, List<Student>>();
            var unassignedStudents = new List<Student>();

            foreach (var teacher in teachers)
            {
                teacherAssignments[teacher] = new List<Student>();
            }

            // Сначала распределяем учеников с "редкими" предметами (где меньше учителей)
            var studentsBySubjectAvailability = students
                .OrderBy(s => teachers.Count(t => t.Subjects.Contains(s.Subject)))
                .ToList();

            foreach (var student in studentsBySubjectAvailability)
            {
                var availableTeachers = teachers
                    .Where(t => t.Subjects.Contains(student.Subject) &&
                                teacherAssignments[t].Count < 4)
                    .OrderBy(t => teacherAssignments[t].Count); // Выбираем учителей с минимальной нагрузкой

                var assignedTeacher = availableTeachers.FirstOrDefault();

                if (assignedTeacher != null)
                {
                    teacherAssignments[assignedTeacher].Add(student);
                }
                else
                {
                    unassignedStudents.Add(student);
                }
            }


            return unassignedStudents.Count == 0;
            /*// Вывод результатов
            Console.WriteLine("\nРезультат распределения:");
            foreach (var (teacher, assignedStudents) in teacherAssignments)
            {
                Console.WriteLine($"{teacher.Name} ({string.Join(", ", teacher.Subjects)}) — учеников: {assignedStudents.Count} из 4");
                foreach (var student in assignedStudents)
                {
                    Console.WriteLine($"  → {student.Name} ({student.Subject})");
                }
            }

            if (unassignedStudents.Count > 0)
            {
                Console.WriteLine("\n❌ Не распределены:");
                foreach (var student in unassignedStudents)
                {
                    Console.WriteLine($"- {student.Name} ({student.Subject})");
                }
                return false;
            }

            Console.WriteLine("\nВсе ученики распределены оптимально.");
            return true;*/
        }
    }
}

