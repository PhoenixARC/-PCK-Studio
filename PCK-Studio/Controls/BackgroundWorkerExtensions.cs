using System.ComponentModel;
using System.Diagnostics;

namespace PckStudio.Controls
{
    static class BackgroundWorkerExtensions
    {
        public static void ReportProgressInfo(this BackgroundWorker worker, string message) 
            => worker.ReportProgress(0, ProgressReportMessage.Info(message));
        public static void ReportProgressWarning(this BackgroundWorker worker, string message) 
            => worker.ReportProgress(0, ProgressReportMessage.Warning(message));
        public static void ReportProgressError(this BackgroundWorker worker, string message) 
            => worker.ReportProgress(0, ProgressReportMessage.Error(message));
        [Conditional("DEBUG")]
        public static void ReportProgressDebug(this BackgroundWorker worker, string message) 
            => worker.ReportProgress(0, ProgressReportMessage.Debug(message));
    }
}
