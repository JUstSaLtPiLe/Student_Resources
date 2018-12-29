using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class Clazz
    {
        public int ClazzId { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public List<StudentClazz> ListStudentClazz { get; set; }
        public List<ClazzSubject> ListClazzSubject { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public ClazzStatus Status { get; set; }
    }
    public enum ClazzStatus
    {
        Active = 1,
        Deactive = 0
    }
}
