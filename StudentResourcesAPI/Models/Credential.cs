using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentResourcesAPI.Models
{
    public class Credential
    {
        public Credential()
        {

        }

        public Credential(long OwnerId)
        {
            this.AccessToken = Guid.NewGuid().ToString();
            this.RefreshToken = Guid.NewGuid().ToString();
            this.OwnerId = OwnerId;
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
            this.Status = CredentialStatus.Active;
            this.ExpiredAt = DateTime.Now.AddDays(7);
        }
        [Key]
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public CredentialStatus Status { get; set; }

        public bool IsValid()
        {
            return this.Status != CredentialStatus.Deactive && this.ExpiredAt > DateTime.Now;
        }
    }

    public enum CredentialStatus
    {
        Active = 1,
        Deactive = 0
    }
}
