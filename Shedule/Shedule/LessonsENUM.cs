using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shedule
{
    public enum Lessons
    {
        [Display(Name = "математика")]
        Math,
        [Display(Name = "физика")]
        Physic,
        [Display(Name = "информатика")]
        Informatic
    }
}
