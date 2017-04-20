using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openMIC.Model
{
    public class Vendor
    {
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [AcronymValidation]
        [Searchable]
        public string Acronym
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [Searchable]
        public string Name
        {
            get;
            set;
        }

        [Label("Phone Number")]
        [StringLength(200)]
        public string PhoneNumber
        {
            get;
            set;
        }

        [Label("E-Mail")]
        [StringLength(200)]
        public string ContactEmail
        {
            get;
            set;
        }

        [Label("Web Page")]
        [UrlValidation]
        public string URL
        {
            get;
            set;
        }

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy
        {
            get;
            set;
        }

        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy
        {
            get;
            set;
        }
    }
}