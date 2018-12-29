using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public List<ClazzSubject> ListClazzSubject { get; set; }
    }
}
