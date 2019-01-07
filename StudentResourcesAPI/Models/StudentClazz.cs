using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class StudentClazz
    {
        public StudentClazz()
        {
            this.Status = StudentClazzStatus.Active;
        }
        public int AccountId { get; set; }
        public int ClazzId { get; set; }
        public Account Account { get; set; }
        public Clazz Clazz { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime GraduateDate { get; set; }
        public StudentClazzStatus Status { get; set; }
    }

    public enum StudentClazzStatus
    {
        Active = 1,
        Deactive = 0
    }
}
