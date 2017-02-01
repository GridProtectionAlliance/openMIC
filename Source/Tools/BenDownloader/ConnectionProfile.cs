using System;
using GSF.Data.Model;

namespace BenDownloader
{
    public class ConnectionProfile
    {
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

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

        public string UpdatedBy
        {
            get;
            set;
        }
    }
}