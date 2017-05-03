namespace openMIC.Model
{
    public enum ProgressState
    {
        Processing,
        Skipped,
        Succeeded,
        Failed,
        Finished
    }

    public class ProgressUpdate
    {
        public ProgressUpdate()
        {            
        }

        public ProgressUpdate(ProgressState state, bool targetIsOverall, string progressMessage, long progressComplete, long progressTotal)
        {
            State = state;
            TargetIsOverall = targetIsOverall;
            ProgressMessage = progressMessage;
            ProgressComplete = progressComplete;
            ProgressTotal = progressTotal;
        }

        public bool TargetIsOverall
        {
            get;
            set;
        }

        public ProgressState State
        {
            get;
            set;
        }

        public string DeviceName
        {
            get;
            set;
        }

        public long ValuesProcessed
        {
            get;
            set;
        }

        public long FilesDownloaded
        {
            get;
            set;
        }

        public long TotalFilesDownloaded
        {
            get;
            set;
        }

        public long ProgressComplete
        {
            get;
            set;
        }

        public long ProgressTotal
        {
            get;
            set;
        }

        public string ProgressMessage
        {
            get;
            set;
        }
    }
}