namespace GolBet.Services.Helpers;

/// <summary>
/// Conversiones entre hora local de Colombia y UTC
/// para la capa de servicios.
/// </summary>
public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo ColombiaZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");

    /// <summary>
    /// Convierte una fecha UTC de la base de datos
    /// a hora local de Colombia.
    /// </summary>
    public static DateTime ToColombiaTime(this DateTime utc)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utc, DateTimeKind.Utc),
            ColombiaZone);
    }

    /// <summary>
    /// Convierte una fecha local de Colombia
    /// a UTC para guardarla en la base de datos.
    /// </summary>
    public static DateTime ToUtcFromColombia(
        this DateTime colombiaLocal)
    {
        return TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                colombiaLocal,
                DateTimeKind.Unspecified),
            ColombiaZone);
    }
}
