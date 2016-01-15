using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("UserAccount")]
    public class UserAccount
    {
        public Guid ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Name
        {
            get;
            set;
        }

        [StringLength(200)]
        public string Password
        {
            get;
            set;
        }

        [StringLength(200)]
        public string FirstName
        {
            get;
            set;
        }

        [StringLength(200)]
        public string LastName
        {
            get;
            set;
        }

        public Guid DefaultNodeID
        {
            get;
            set;
        }

        [StringLength(200)]
        public string Phone
        {
            get;
            set;
        }

        [StringLength(200)]
        public string Email
        {
            get;
            set;
        }

        public bool LockedOut
        {
            get;
            set;
        }

        public bool UseADAuthentication
        {
            get;
            set;
        }

        public DateTime? ChangePasswordOn
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string CreatedBy
        {
            get;
            set;
        }

        public DateTime UpdatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string UpdatedBy
        {
            get;
            set;
        }
    }
}