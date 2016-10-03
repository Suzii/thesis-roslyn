using CMS.EventLog;

namespace SampleTestProject.CsSamples
{
    public class BH1001LogEventArguments
    {
        public void SampleMethod()
        {
            EventLogProvider.LogEvent("I", "source", "eventCode", "eventDescription");
            EventLogProvider.LogEvent("E", "source", "eventCode", "eventDescription");
            EventLogProvider.LogEvent("W", "source", "eventCode", "eventDescription");
            EventLogProvider.LogEvent("S", "source", "eventCode", "eventDescription");
            EventLogProvider.LogEvent(EventType.ERROR, "source", "eventCode", "eventDescription");
            EventLogProvider.LogEvent(EventType.INFORMATION, "source", "eventCode", "eventDescription");
            EventLogProvider.LogEvent(EventType.WARNING, "source", "eventCode", "eventDescription");
        }
    }
}