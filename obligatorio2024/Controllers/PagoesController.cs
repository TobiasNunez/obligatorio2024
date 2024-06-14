using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

namespace obligatorio2024.Controllers
{
    namespace obligatorio2024.Controllers
    {
        public class PagoesController : Controller
        {
            private readonly Obligatorio2024Context _context;

            public PagoesController(Obligatorio2024Context context)
            {
                _context = context;
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
                ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
                return View();
            }

            // POST: Pagoes/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Id,ReservaId,Monto,FechaPago,MetodoPago,Moneda,TipoCambio")] Pago pago)
            {
                if (ModelState.IsValid)
                {
                    // Obtener la reserva y el cliente asociado para aplicar el descuento
                    var reserva = await _context.Reservas
                        .Include(r => r.Cliente)
                        .FirstOrDefaultAsync(r => r.Id == pago.ReservaId);

                    if (reserva != null && reserva.Cliente != null)
                    {
                        pago.Monto = AplicarDescuento(pago.Monto, reserva.Cliente.TipoCliente);
                    }

                    _context.Add(pago);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
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
                ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
                return View(pago);
            }

            // POST: Pagoes/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,ReservaId,Monto,FechaPago,MetodoPago,Moneda,TipoCambio")] Pago pago)
            {
                if (id != pago.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
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
                ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
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

}
