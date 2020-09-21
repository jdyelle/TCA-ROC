using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Diagnostics.Tracing;

namespace ODL.Common
{
    public class LogHandler : Microsoft.Extensions.Logging.ILogger
    {
        public bool DebugMode = false;
        public DataTable LogRecords = null;  
        public event EventHandler<Boolean> LogTableUpdated;

        public LogHandler(bool DebugMode = false)
        {
            this.DebugMode = DebugMode;
            this.LogRecords = new DataTable("LogRecords");
            LogRecords.Columns.Add("Timestamp", typeof(DateTime));
            LogRecords.Columns.Add("LogLevel", typeof(String));
            LogRecords.Columns.Add("Message", typeof(String));
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            
            DataRow _row = LogRecords.NewRow();
            _row.ItemArray = new object[] { DateTime.Now, logLevel, formatter(state, exception) };
            LogRecords.Rows.Add(_row);
            LogTableUpdated(null, true);
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
