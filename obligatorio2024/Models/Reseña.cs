using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Reseña
{
    public int Id { get; set; }

    public int? ClienteId { get; set; }

    public int? RestauranteId { get; set; }

    public int Puntaje { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaReseña { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Restaurante? Restaurante { get; set; }
}
