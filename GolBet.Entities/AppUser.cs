using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GolBet.Entities;

/// <summary>
/// Usuario de la aplicación GolBet.
/// Hereda de IdentityUser para obtener login,
/// contraseña, correo y roles.
/// </summary>
public class AppUser : IdentityUser
{
    [MaxLength(100)]
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Saldo virtual del usuario.
    /// </summary>
    public decimal Balance { get; set; }

    public ICollection<Bet> Bets { get; set; } =
        new List<Bet>();
}
