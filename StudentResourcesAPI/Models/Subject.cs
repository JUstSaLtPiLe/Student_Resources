using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class Subject
    {
        public Subject()
        {
            this.Status = SubjectStatus.Active;
        }
        [Key]
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public SubjectStatus Status { get; set; }
        public List<ClazzSubject> ListClazzSubject { get; set; }
    }

    public enum SubjectStatus
    {
        Active = 1,
        Deactive = 0
    }
}
