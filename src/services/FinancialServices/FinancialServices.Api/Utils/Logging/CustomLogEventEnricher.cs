using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Api.Utils.Logging
{
    public class CustomLogEventEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var current = Activity.Current;

            if (current != null)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("traceId", current.TraceId.ToString()));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("spanId", current.SpanId.ToString()));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("parentSpanId", current.ParentSpanId.ToString()));
            }
        }
    }
}
