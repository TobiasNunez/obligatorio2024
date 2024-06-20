using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;
using obligatorio2024.Service;

namespace obligatorio2024.Controllers
{
    public class PagoesController : Controller
    {
        private readonly Obligatorio2024Context _context;
        private readonly PagoService _pagoService;

        public PagoesController(Obligatorio2024Context context, PagoService pagoService)
        {
            _context = context;
            _pagoService = pagoService;
        }

        // GET: Pagoes
        public async Task<IActionResult> Index()
        {
            var obligatorio2024Context = _context.Pagos.Include(p => p.Reserva).ThenInclude(r => r.Cliente);
            return View(await obligatorio2024Context.ToListAsync());
        }

        // GET: Pagoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Pagoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pagoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaId,Monto,MetodoPago,Moneda")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                // Verificar si la reserva existe
                var reserva = await _context.Reservas.FindAsync(pago.ReservaId);
                if (reserva == null)
                {
                    ModelState.AddModelError("ReservaId", "La reserva no existe.");
                    return View(pago);
                }

                // Verificar si ya existe un pago para la misma reserva
                var existingPago = await _context.Pagos
                    .FirstOrDefaultAsync(p => p.ReservaId == pago.ReservaId && p.Id != pago.Id);
                if (existingPago != null)
                {
                    ModelState.AddModelError("ReservaId", "Ya existe un pago para esta reserva.");
                    return View(pago);
                }

                var nuevoPago = await _pagoService.CrearPagoAsync(pago.Monto, pago.Moneda, pago.MetodoPago);
                nuevoPago.ReservaId = pago.ReservaId;
                nuevoPago.FechaPago = DateTime.Now;

                if (reserva.Cliente != null)
                {
                    nuevoPago.Monto = AplicarDescuento(nuevoPago.Monto, reserva.Cliente.TipoCliente);
                }

                _context.Add(nuevoPago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pago);
        }

        // GET: Pagoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // POST: Pagoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Monto,MetodoPago,Moneda,TipoCambio")] Pago pago)
        {
            if (id != pago.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el pago original para mantener las propiedades que no se modifican desde el formulario
                    var pagoOriginal = await _context.Pagos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (pagoOriginal == null)
                    {
                        return NotFound();
                    }

                    // Mantener el ReservaId original y la FechaPago
                    pago.ReservaId = pagoOriginal.ReservaId;
                    pago.FechaPago = pagoOriginal.FechaPago;

                    // Obtener la reserva y el cliente asociado para aplicar el descuento
                    var reserva = await _context.Reservas
                        .Include(r => r.Cliente)
                        .FirstOrDefaultAsync(r => r.Id == pago.ReservaId);

                    if (reserva != null && reserva.Cliente != null)
                    {
                        pago.Monto = AplicarDescuento(pago.Monto, reserva.Cliente.TipoCliente);
                    }

                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pago);
        }

        // GET: Pagoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }

        private decimal AplicarDescuento(decimal monto, string tipoCliente)
        {
            switch (tipoCliente)
            {
                case "Frecuente":
                    return monto * 0.90m; // 10% de descuento
                case "VIP":
                    return monto * 0.80m; // 20% de descuento
                default:
                    return monto; // Sin descuento para nuevos clientes
            }
        }
    }
}




