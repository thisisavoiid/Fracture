public class TimeMSDisplay : Display<TimeMS>
{
    protected override string FormatValue(TimeMS value)
    {
        return $"{(int)value.Minutes:D2} : {(int)value.Seconds:D2}";
    }
}