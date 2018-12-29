using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public RoleStatus Status { get; set; }
    }
    public enum RoleStatus
    {
        Active = 1,
        Deactive = 0
    }
}
