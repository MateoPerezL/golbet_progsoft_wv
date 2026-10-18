using System.ComponentModel.DataAnnotations;

namespace GolBet.Web.Models;

public class RegisterViewModel
{
    [Required(
        ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(
        100,
        ErrorMessage =
            "El nombre no puede superar los 100 caracteres")]
    [Display(Name = "Nombre completo")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(
        ErrorMessage = "Ingresa un correo válido")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(
        100,
        MinimumLength = 6,
        ErrorMessage =
            "La contraseña debe tener mínimo 6 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = null!;

    [Required(
        ErrorMessage =
            "Debes confirmar la contraseña")]
    [DataType(DataType.Password)]
    [Compare(
        "Password",
        ErrorMessage =
            "Las contraseñas no coinciden")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = null!;
}
