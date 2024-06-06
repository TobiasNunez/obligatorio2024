using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace obligatorio2024.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    [Required]
    [RegularExpression("Nuevo|Frecuente|VIP", ErrorMessage = "El tipo de cliente debe ser 'Nuevo', 'Frecuente' o 'VIP'.")]
    public string TipoCliente { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();
}
