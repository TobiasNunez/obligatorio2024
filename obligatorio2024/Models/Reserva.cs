﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace obligatorio2024.Models;

public partial class Reserva
{
    public int Id { get; set; }

    public int? ClienteId { get; set; }

    public int? MesaId { get; set; }

    public DateTime FechaReserva { get; set; }
    [Required]
    [RegularExpression("Pendiente|Confirmada|Cancelada", ErrorMessage = "El estado de la reserva debe ser 'Pendiente', 'Confirmada' o 'Cancelada'.")]
    public string Estado { get; set; } = null!;

    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<Clima> Climas { get; set; } = new List<Clima>();

    public virtual Mesa? Mesa { get; set; }

    public virtual ICollection<Ordene> Ordenes { get; set; } = new List<Ordene>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
