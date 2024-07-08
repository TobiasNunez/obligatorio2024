using obligatorio2024.Models;
using System.Threading.Tasks;

namespace obligatorio2024.Service
{
    public class PagoService
    {
        private readonly CurrencyService _currencyService;

        public PagoService(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public async Task<(Pago, Cotizacion)> CrearPagoAsync(decimal monto, string moneda, string metodoPago)
        {
            var pago = new Pago
            {
                Monto = monto,
                MetodoPago = metodoPago,
                Moneda = moneda,
                FechaPago = DateTime.Now
            };

            decimal coti = 0;

            if (moneda != "UYU")
            {
                var tipoCambio = await _currencyService.GetExchangeRate("UYU", moneda);
                if (tipoCambio.HasValue)
                {
                    coti = tipoCambio.Value;
                    pago.Monto = monto * tipoCambio.Value; // Calcula el monto en UYU
                }
                else
                {
                    throw new ArgumentException("No se pudo obtener la tasa de cambio.");
                }
            }
            else
            {
                coti = 1; // El tipo de cambio es 1 para UYU
            }

            var cotizacion = new Cotizacion
            {
                Cotizacion1 = coti,
                Moneda = moneda,
                PagosId = pago.Id // Asignamos el ID del pago después de guardar el pago
            };

            return (pago, cotizacion);
        }
    }
}
