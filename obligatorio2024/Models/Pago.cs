using obligatorio2024.Service;
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

    public static async Task<Pago> CrearPagoAsync(decimal monto, string moneda, string metodoPago, CurrencyService currencyService)
    {
        var pago = new Pago
        {
            Monto = monto,
            FechaPago = DateTime.Now,
            MetodoPago = metodoPago,
            Moneda = moneda
        };

        if (moneda != "UYU")
        {
            decimal? tipoCambio = await currencyService.GetExchangeRate("UYU", moneda);
            pago.TipoCambio = tipoCambio;
            if (tipoCambio.HasValue)
            {
                pago.Monto = monto * tipoCambio.Value; // Calcula el monto en UYU
            }
        }
        else
        {
            pago.TipoCambio = 1; // El tipo de cambio es 1 para UYU
        }

        return pago;
    }
}
