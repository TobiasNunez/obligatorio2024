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


        public async Task<(Pago pago, Cotizacion cotizacion)> CrearPagoAsync(decimal monto, string moneda, string metodoPago)
        {
            var pago = new Pago
            {
                Monto = monto,
                MetodoPago = metodoPago,
                Moneda = moneda,
                FechaPago = DateTime.Now
            };

            // Obtenemos la tasa de cambio en relación a UYU
            var tipoCambio = await _currencyService.GetExchangeRate("UYU", moneda);
            if (tipoCambio.HasValue)
            {
                pago.TipoCambio = tipoCambio.Value;
                pago.Monto = monto * tipoCambio.Value; // Calcula el monto en UYU
            }

            var cotizacion = new Cotizacion
            {
                Cotizacion1 = tipoCambio ?? 1,
                Moneda = moneda,
                PagosId = pago.Id // Asignamos el ID del pago después de guardar el pago
            };

            return (pago, cotizacion);
        }


    }
}
