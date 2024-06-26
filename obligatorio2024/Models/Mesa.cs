﻿using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Mesa
{
    public int Id { get; set; }

    public int NumeroMesa { get; set; }

    public int Capacidad { get; set; }

    public string Estado { get; set; } = null!;

    public int? RestauranteId { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual Restaurante? Restaurante { get; set; }
}
