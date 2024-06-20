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

        public async Task<Pago> CrearPagoAsync(decimal monto, string moneda, string metodoPago)
        {
            return await Pago.CrearPagoAsync(monto, moneda, metodoPago, _currencyService);
        }
    }
}
