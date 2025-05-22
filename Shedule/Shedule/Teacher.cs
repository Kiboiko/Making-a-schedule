using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public class Teacher:Person
    {
        public List<Lessons> Subjects = new List<Lessons>();
        public int Priority;
        private int _countStudents;
        public int CountStudents => _countStudents;
        public Teacher(string name, string startOfStudyTime, string endOfStudyTime, List<Lessons> _lessons,int priority)
            :base(name,startOfStudyTime,endOfStudyTime) { 
            Subjects = _lessons;
            Priority = priority;
        }

        public bool TryAddStudent()
        {
            if (_countStudents >= 4)
                return false;
        
            _countStudents++;
            return true;
        }

        // Метод для уменьшения счетчика (если ученик открепляется)
        public void RemoveStudent()
        {
            if (_countStudents > 0)
                _countStudents--;
        }

        public override string ToString()
        {
            return $"Имя: {Name}\nКласс: Преподователь\n" +
                $"Предметы: {string.Join(',', Subjects)}\nПриоритет: {Priority}\n" +
                $"Время начала:{StartOfStudyingTime.ToString("HH:mm")}\n" +
                $"Время конца:{EndOfStudyingTime.ToString("HH:mm")}";
        }
    }
}
