using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class Account
    {
        public Account()
        {
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
            this.Status = AccountStatus.Active;
            this.Salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(this.Salt);
            }
        }

        public void EncryptPassword(string password)
        {
            this.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: this.Salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        }
        [Key]
        public long Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public byte[] Salt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AccountRole Role { get; set; }
        public AccountStatus Status { get; set; }
        public GeneralInformation GeneralInformation { get; set; }
    }
    public enum AccountRole
    {
        Student = 1,
        Employee = 2
    }

    public enum AccountStatus
    {
        Active = 1,
        Deactive = 0
    }
}
