using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;
using obligatorio2024.Service;

namespace obligatorio2024.Controllers
{
    [Authorize(Policy = "VerPagosPermiso")]
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
        public async Task<IActionResult> Index(int? restauranteId)
        {
            var restaurantes = await _context.Restaurantes.ToListAsync();
            ViewBag.RestauranteId = new SelectList(restaurantes, "Id", "Dirección", restauranteId ?? 1);
            ViewBag.SelectedRestauranteId = restauranteId ?? 1;

            var pagos = _context.Pagos
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Cliente)
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Mesa)
                    .ThenInclude(m => m.Restaurante)
                .AsQueryable();

            if (restauranteId.HasValue)
            {
                pagos = pagos.Where(p => p.Reserva.Mesa.RestauranteId == restauranteId.Value);
            }
            else
            {
                pagos = pagos.Where(p => p.Reserva.Mesa.RestauranteId == 1);
            }

            return View(await pagos.ToListAsync());
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
                var reserva = await _context.Reservas.FindAsync(pago.ReservaId);
                if (reserva == null)
                {
                    ModelState.AddModelError("ReservaId", "La reserva no existe.");
                    return View(pago);
                }

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
                    var pagoOriginal = await _context.Pagos.FindAsync(id);
                    if (pagoOriginal == null)
                    {
                        return NotFound();
                    }

                    var nuevoPago = await _pagoService.CrearPagoAsync(pago.Monto, pago.Moneda, pago.MetodoPago);

                    pagoOriginal.Monto = nuevoPago.Monto;
                    pagoOriginal.MetodoPago = pago.MetodoPago;
                    pagoOriginal.Moneda = pago.Moneda;
                    pagoOriginal.TipoCambio = nuevoPago.TipoCambio;

                    pagoOriginal.ReservaId = pagoOriginal.ReservaId;
                    pagoOriginal.FechaPago = pagoOriginal.FechaPago;

                    var reserva = await _context.Reservas
                        .Include(r => r.Cliente)
                        .FirstOrDefaultAsync(r => r.Id == pagoOriginal.ReservaId);

                    if (reserva != null && reserva.Cliente != null)
                    {
                        pagoOriginal.Monto = AplicarDescuento(pagoOriginal.Monto, reserva.Cliente.TipoCliente);
                    }

                    _context.Update(pagoOriginal);
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
                    return monto * 0.90m;
                case "VIP":
                    return monto * 0.80m;
                default:
                    return monto;
            }
        }
    }
}
