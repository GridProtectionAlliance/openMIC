using GSF.Data.Model;

namespace openMIC.Model
{
    public class Runtime
    {
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        public int SourceID
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}