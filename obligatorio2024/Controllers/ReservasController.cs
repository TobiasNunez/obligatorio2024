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
    public class ReservasController : Controller
    {
        private readonly Obligatorio2024Context _context;
        private readonly WeatherService _weatherService;

        public ReservasController(Obligatorio2024Context context, WeatherService weatherService)
        {
            _context = context;
            _weatherService = weatherService;
        }

        // GET: Reservas
        public async Task<IActionResult> Index(int? restauranteId, string emailCliente)
        {
            var restaurantes = await _context.Restaurantes.ToListAsync();
            ViewBag.RestauranteId = new SelectList(restaurantes, "Id", "Dirección", restauranteId ?? 1);
            ViewBag.SelectedRestauranteId = restauranteId ?? 1;
            ViewBag.EmailCliente = emailCliente;

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

            if (!string.IsNullOrEmpty(emailCliente))
            {
                // Buscar el cliente por su email
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == emailCliente);
                if (cliente != null)
                {
                    // Filtrar las reservas por el ID del cliente
                    reservas = reservas.Where(r => r.ClienteId == cliente.Id);
                }
                else
                {
                    // Manejar el caso cuando no se encuentra un cliente con el email especificado
                    // Aquí, simplemente devolveremos un conjunto vacío de reservas.
                    return View(Enumerable.Empty<Reserva>());
                }
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

        public IActionResult Create()
        {
            ViewData["MesaId"] = new SelectList(_context.Mesas.Where(m => m.Estado == "Disponible").Include(m => m.Restaurante).Select(m => new {
                m.Id,
                Display = m.NumeroMesa + " - " + m.Restaurante.Dirección
            }), "Id", "Display");
            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string emailCliente, [Bind("MesaId,FechaReserva,Estado")] Reserva reserva)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == emailCliente);
            if (cliente == null)
            {
                ViewBag.ErrorMessage = "No se encontró un cliente con este email.";
                ViewData["MesaId"] = new SelectList(_context.Mesas.Where(m => m.Estado == "Disponible").Include(m => m.Restaurante).Select(m => new {
                    m.Id,
                    Display = m.NumeroMesa + " - " + m.Restaurante.Dirección
                }), "Id", "Display", reserva.MesaId);
                return View(reserva);
            }

            if (ModelState.IsValid)
            {
                reserva.ClienteId = cliente.Id;
                _context.Add(reserva);

                var mesa = await _context.Mesas.FindAsync(reserva.MesaId);
                if (mesa != null)
                {
                    mesa.Estado = "Reservada";
                    _context.Update(mesa);
                }

                // Guardar la reserva para obtener su ID
                await _context.SaveChangesAsync();

                // Llamar a la API del clima y crear la entrada de clima
                var restaurante = await _context.Restaurantes.FirstOrDefaultAsync(r => r.Id == mesa.RestauranteId);
                if (restaurante != null)
                {
                    await CreateClimaEntryAsync(reserva.FechaReserva, reserva.Id, restaurante.Ciudad);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            ViewBag.EmailCliente = reserva.Cliente?.Email;
            ViewData["MesaId"] = new SelectList(_context.Mesas.Where(m => m.Estado == "Disponible" || m.Id == reserva.MesaId).Include(m => m.Restaurante).Select(m => new {
                m.Id,
                Display = m.NumeroMesa + " - " + m.Restaurante.Dirección
            }), "Id", "Display", reserva.MesaId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string emailCliente, [Bind("Id,MesaId,FechaReserva,Estado")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == emailCliente);
            if (cliente == null)
            {
                ViewBag.ErrorMessage = "No se encontró un cliente con este email.";
                ViewData["MesaId"] = new SelectList(_context.Mesas.Where(m => m.Estado == "Disponible" || m.Id == reserva.MesaId).Include(m => m.Restaurante).Select(m => new {
                    m.Id,
                    Display = m.NumeroMesa + " - " + m.Restaurante.Dirección
                }), "Id", "Display", reserva.MesaId);
                return View(reserva);
            }

            if (ModelState.IsValid)
            {
                reserva.ClienteId = cliente.Id;
                try
                {
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

        private async Task CreateClimaEntryAsync(DateTime fechaReserva, int reservaId, string ciudad)
        {
            var weather = await _weatherService.GetWeatherAsync(ciudad);

            var clima = new Clima
            {
                Fecha = fechaReserva,
                Temperatura = (decimal)weather.Main.Temp.Value, // Conversión explícita de double a decimal
                DescripciónClima = weather.Weather.First().Description,
                ReservaId = reservaId
            };

            _context.Climas.Add(clima);
            await _context.SaveChangesAsync();
        }
    }
}
