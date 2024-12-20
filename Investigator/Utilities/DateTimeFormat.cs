namespace Investigator.Utilities
{
    public class DateTimeFormat
    {
        public static string FormatString(DateTime data)
        {
            int year = data.Year;
            int month = data.Month;
            int day = data.Day;
            int hour = data.Hour;
            int minute = data.Minute;
            int second = data.Second;
            string dataDay = data.DayOfWeek.ToString();
            return $"{dataDay}, {year}-{month}-{day} {hour}:{minute}:{second}";
        }
    }
}
