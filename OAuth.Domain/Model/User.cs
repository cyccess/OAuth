using System;
using System.Collections.Generic;


namespace OAuth.Domain.Model
{
    public class User : AggregateRoot
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string DigitalCertificate { get; set; }

        public DateTime? LastLogonDate { get; set; }

        public byte Status { get; set; }

        public DateTime AddDate { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<UserProject> UserProjects { get; set; }
    }
}
