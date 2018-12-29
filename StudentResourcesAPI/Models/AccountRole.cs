using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class AccountRole
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public Account Account { get; set; }
        public Role Role { get; set; }
        public DateTime GrantDate { get; set; }
    }
}
