using Serilog.Core;
using Serilog.Events;

namespace ObjectLogging.Logging
{
    public class ColumnsEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.ContainsKey("LogData") && logEvent.Properties["LogData"] is StructureValue logData)
            {
                foreach (var property in logData.Properties)
                {
                    logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(property.Name, property.Value.ToString("l", null)));
                }
            }
        }
    }
}