using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Core.IO.Java
{
    public class ImportStatusReport
    {
#if DEBUG
        public static ImportStatusReport Debug = new ImportStatusReport(System.Diagnostics.Debug.Listeners);
#endif
#if TRACE
        public static ImportStatusReport Trace = new ImportStatusReport(System.Diagnostics.Trace.Listeners);
#endif

        private readonly TraceListener[] _listeners;

        private ImportStatusReport(TraceListenerCollection listeners)
        {
            _listeners = new TraceListener[listeners.Count];
            listeners.CopyTo(_listeners, 0);
        }

        private ImportStatusReport(params TraceListener[] listeners)
        {
            _listeners = listeners;
        }

        private class ImportStatusReportTextWriter : TextWriter
        {
            private readonly Action<string> _postMessage;

            public override Encoding Encoding => Encoding.Default;

            public ImportStatusReportTextWriter(Action<string> postMessage) => _postMessage = postMessage;

            public override void Write(string value) => _postMessage(value);
            public override void WriteLine(string value) => _postMessage(value + Environment.NewLine);
        }

        public static ImportStatusReport CreateCustom(Action<string> postMessage)
        {
            TextWriterTraceListener listener = new TextWriterTraceListener(new ImportStatusReportTextWriter(postMessage));
            return new ImportStatusReport(listener);
        }

        public static ImportStatusReport CreateEmpty()
        {
            return new ImportStatusReport();
        }

        public void Post(string message)
        {
            foreach (TraceListener listener in _listeners)
            {
                listener.WriteLine(message);
            }
        }
    }
}
