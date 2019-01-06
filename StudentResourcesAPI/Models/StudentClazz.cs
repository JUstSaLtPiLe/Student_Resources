using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class StudentClazz
    {
        public int AccountId { get; set; }
        public int ClazzId { get; set; }
        public Account Account { get; set; }
        public Clazz Clazz { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime GraduateDate { get; set; }
        public int Status { get; set; }
    }
}
