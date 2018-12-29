using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class GeneralInformation
    {
        [Key]
        public int AccountId { get; set; }
        public string Name { get; set; }
        public UserGender Gender { get; set; }
        [Display(Name = "Birthday")]
        public DateTime Dob { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public Account Account { get; set; }
        public enum UserGender
        {
            Female = 0,
            Male = 1,
            Other = 2
        }
    }
}
