using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class Grade
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public float GradeNumber { get; set; }
        public int GradeType { get; set; }
        public GradeStatus Status { get; set; }
    }

    public enum GradeStatus
    {
        Active = 1,
        Deactive = 0,
    }
}
