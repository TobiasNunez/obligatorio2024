using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Pago
{
    public int Id { get; set; }

    public int? ReservaId { get; set; }

    public decimal Monto { get; set; }

    public DateTime FechaPago { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string Moneda { get; set; } = null!;

    public decimal? TipoCambio { get; set; }

    public virtual Reserva? Reserva { get; set; }
}
