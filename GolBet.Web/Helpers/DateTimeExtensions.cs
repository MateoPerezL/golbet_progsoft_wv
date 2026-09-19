namespace GolBet.Web.Helpers;

public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo ColombiaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");

    public static DateTime ToColombiaTime(this DateTime utcDate)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcDate, DateTimeKind.Utc),
            ColombiaZone);
    }
}
