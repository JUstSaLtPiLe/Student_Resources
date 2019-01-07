using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class GeneralInformationWithRoles
    {
        public GeneralInformation GeneralInformation { get; set; }
        public int[] RoleIds { get; set; }
        public int AccountId { get; set; }
        public string Password { get; set; }
        public AccountStatus Status { get; set; }
    }
}
