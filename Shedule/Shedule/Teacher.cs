using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public class Teacher
    {
        public string Name { get; }
        public List<Lessons> lessons = new List<Lessons>();
        public int Priority;
        public Teacher(string name,List<Lessons> _lessons,int priority) { 
            Name = name;
            lessons = _lessons;
            Priority = priority;
        }

        public override string ToString()
        {
            return $"Имя: {Name}\nПредметы: {string.Join(',', lessons)}\nПриоритет: {Priority}";
        }
    }
}
