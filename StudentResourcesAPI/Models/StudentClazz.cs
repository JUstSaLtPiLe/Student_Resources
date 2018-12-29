using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class StudentClazz
    {
        public int StudentId { get; set; }

        public int ClazzId { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime GraduateDate { get; set; }

        public int Status { get; set; }
    }
}
