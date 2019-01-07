using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class RoleAccount
    {
        public RoleAccount()
        {
            this.Status = RoleAccountStatus.Active;
            this.GrantDate = DateTime.Now;
        }
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public Account Account { get; set; }
        public Role Role { get; set; }
        public DateTime GrantDate { get; set; }
        public RoleAccountStatus Status { get; set; }
    }

    public enum RoleAccountStatus
    {
        Active = 1,
        Deactive = 0
    }
}
