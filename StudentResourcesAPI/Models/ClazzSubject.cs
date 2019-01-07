using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class ClazzSubject
    {
        public ClazzSubject()
        {
            this.Status = ClazzSubjectStatus.Active;
        }
        public int ClazzId { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public Clazz Clazz { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ClazzSubjectStatus Status { get; set; }
    }

    public enum ClazzSubjectStatus
    {
        Active = 1,
        Deactive = 0
    }
}
