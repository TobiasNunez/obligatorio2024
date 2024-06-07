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
    public class ReservasController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public ReservasController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index(int? restauranteId, int? clienteId)
        {
            var restaurantes = await _context.Restaurantes.ToListAsync();
            ViewBag.RestauranteId = new SelectList(restaurantes, "Id", "Dirección", restauranteId ?? 1);
            ViewBag.SelectedRestauranteId = restauranteId ?? 1;
            ViewBag.ClienteId = clienteId;

            var reservas = _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .ThenInclude(m => m.Restaurante)
                .AsQueryable();

            if (restauranteId.HasValue)
            {
                reservas = reservas.Where(r => r.Mesa.RestauranteId == restauranteId.Value);
            }
            else
            {
                reservas = reservas.Where(r => r.Mesa.RestauranteId == 1);
            }

            if (clienteId.HasValue)
            {
                reservas = reservas.Where(r => r.ClienteId == clienteId.Value);
            }

            return View(await reservas.OrderBy(r => r.FechaReserva).ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["MesaId"] = new SelectList(_context.Mesas.Where(m => m.Estado == "Disponible").Include(m => m.Restaurante).Select(m => new {
                m.Id,
                Display = m.NumeroMesa + " - " + m.Restaurante.Dirección
            }), "Id", "Display");
            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,MesaId,FechaReserva,Estado")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);

                var mesa = await _context.Mesas.FindAsync(reserva.MesaId);
                if (mesa != null)
                {
                    mesa.Estado = "Reservada";
                    _context.Update(mesa);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reserva.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas.Where(m => m.Estado == "Disponible").Include(m => m.Restaurante).Select(m => new {
                m.Id,
                Display = m.NumeroMesa + " - " + m.Restaurante.Dirección
            }), "Id", "Display", reserva.MesaId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", reserva.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas.Where(m => m.Estado == "Disponible" || m.Id == reserva.MesaId).Include(m => m.Restaurante).Select(m => new {
                m.Id,
                Display = m.NumeroMesa + " - " + m.Restaurante.Dirección
            }), "Id", "Display", reserva.MesaId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,MesaId,FechaReserva,Estado")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar el estado de la mesa si cambia
                    var originalReserva = await _context.Reservas.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                    if (originalReserva != null && originalReserva.MesaId != reserva.MesaId)
                    {
                        var originalMesa = await _context.Mesas.FindAsync(originalReserva.MesaId);
                        if (originalMesa != null)
                        {
                            originalMesa.Estado = "Disponible";
                            _context.Update(originalMesa);
                        }

                        var newMesa = await _context.Mesas.FindAsync(reserva.MesaId);
                        if (newMesa != null)
                        {
                            newMesa.Estado = "Reservada";
                            _context.Update(newMesa);
                        }
                    }

                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", reserva.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas.Where(m => m.Estado == "Disponible" || m.Id == reserva.MesaId).Include(m => m.Restaurante).Select(m => new {
                m.Id,
                Display = m.NumeroMesa + " - " + m.Restaurante.Dirección
            }), "Id", "Display", reserva.MesaId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }

}
