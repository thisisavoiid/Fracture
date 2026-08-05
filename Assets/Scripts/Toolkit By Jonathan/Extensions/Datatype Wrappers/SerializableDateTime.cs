using System;

namespace ToolkitByJonathan
{
    [Serializable]
    public class SerializableDateTime
    {
        public int Years;
        public int Months;
        public int Days;
        public int Hours;
        public int Minutes;
        public int Seconds;

        private const float YEARS_MONTHS_RATIO = 12.0f;
        private const float MONTHS_DAYS_RATIO = 30.417f;
        private const float DAYS_HOURS_RATIO = 24.0f;
        private const float HOURS_MINUTES_RATIO = 60.0f;
        private const float MINUTES_SECONDS_RATIO = 60.0f;

        public float TotalSeconds()
        {
            float total = 0f;

            total += this.Seconds;
            total += this.Minutes * MINUTES_SECONDS_RATIO;
            total += this.Hours * HOURS_MINUTES_RATIO * MINUTES_SECONDS_RATIO;
            total += this.Days * DAYS_HOURS_RATIO * HOURS_MINUTES_RATIO * MINUTES_SECONDS_RATIO;
            total += this.Months * MONTHS_DAYS_RATIO * DAYS_HOURS_RATIO * HOURS_MINUTES_RATIO * MINUTES_SECONDS_RATIO;
            total += this.Years * YEARS_MONTHS_RATIO * MONTHS_DAYS_RATIO * DAYS_HOURS_RATIO * HOURS_MINUTES_RATIO * MINUTES_SECONDS_RATIO;

            return total;
        }

        public float TotalMinutes => TotalSeconds() / MINUTES_SECONDS_RATIO;
        public float TotalHours => TotalMinutes / HOURS_MINUTES_RATIO;
        public float TotalDays => TotalHours / DAYS_HOURS_RATIO;
        public float TotalMonths => TotalDays / MONTHS_DAYS_RATIO;
        public float TotalYears => TotalMonths / YEARS_MONTHS_RATIO;

        public override string ToString()
        {
            return $"SerializableDateTime({this.Years}Y; {this.Months}M; {this.Days}D; {this.Hours}h; {this.Minutes}m; {this.Seconds}s)";
        }
    }
}