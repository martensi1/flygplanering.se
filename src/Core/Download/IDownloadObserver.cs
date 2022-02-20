using System;


namespace FPSE.Core.Download
{
    public interface IDownloadObserver
    {
        void OnDownloadFinished(TaskResult result);
    }
}
