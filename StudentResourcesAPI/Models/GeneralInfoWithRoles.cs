using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class GeneralInfoWithRoles
    {
        public GeneralInformation GeneralInformation { get; set; }
        public int[] RoleIds { get; set; }
    }
}
