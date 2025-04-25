namespace Common.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime DateToMinTime(this DateTime t)
        {
            return DateOnly.FromDateTime(t).ToDateTime(TimeOnly.MinValue);
        }
        public static DateTime DateToMaxTime(this DateTime t)
        {
            return DateOnly.FromDateTime(t).ToDateTime(TimeOnly.MaxValue);
        }
    }
}
