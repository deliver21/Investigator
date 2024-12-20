namespace Investigator.Utilities
{
    public class Interval
    {
        public static string SetInterval(DateTime lastSeen)
        {
            TimeSpan process = DateTime.Now - lastSeen;

            if (process.Days >= 365) return $"{process.Days / 365} Year" + (process.Days > 730 ? "s" : "") + " ago";
            if (process.Days >= 30) return $"{process.Days / 30} Month" + (process.Days > 60 ? "s" : "") + " ago";
            if (process.Days >= 7) return $"{process.Days / 7} Week" + (process.Days > 14 ? "s" : "") + " ago";
            if (process.Days >= 1) return $"{process.Days} Day" + (process.Days > 1 ? "s" : "") + " ago";
            if (process.Hours >= 1) return $"{process.Hours} Hour" + (process.Hours > 1 ? "s" : "") + " ago";
            if (process.Minutes >= 1) return $"{process.Minutes} Minute" + (process.Minutes > 1 ? "s" : "") + " ago";
            if (process.Seconds >= 0) return "Less than a minute ago";
            return "";
        }
    }
}
