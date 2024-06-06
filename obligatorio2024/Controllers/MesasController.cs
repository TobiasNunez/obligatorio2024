using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

public class MesasController : Controller
{
    private readonly Obligatorio2024Context _context;

    public MesasController(Obligatorio2024Context context)
    {
        _context = context;
    }

    // GET: Mesas
    public async Task<IActionResult> Index()
    {
        var obligatorio2024Context = _context.Mesas.Include(m => m.Restaurante);
        return View(await obligatorio2024Context.ToListAsync());
    }

    // GET: Mesas/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mesa = await _context.Mesas
            .Include(m => m.Restaurante)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (mesa == null)
        {
            return NotFound();
        }

        return View(mesa);
    }

    // GET: Mesas/Create
    public IActionResult Create()
    {
        ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id");
        return View();
    }

    // POST: Mesas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,NumeroMesa,Capacidad,Estado,RestauranteId")] Mesa mesa)
    {
        if (ModelState.IsValid)
        {
            mesa.NumeroMesa = _context.Mesas.Max(m => (int?)m.NumeroMesa) + 1 ?? 1;
            _context.Add(mesa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id", mesa.RestauranteId);
        return View(mesa);
    }

    // GET: Mesas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mesa = await _context.Mesas.FindAsync(id);
        if (mesa == null)
        {
            return NotFound();
        }
        ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id", mesa.RestauranteId);
        return View(mesa);
    }

    // POST: Mesas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroMesa,Capacidad,Estado,RestauranteId")] Mesa mesa)
    {
        if (id != mesa.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(mesa);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MesaExists(mesa.Id))
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
        ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id", mesa.RestauranteId);
        return View(mesa);
    }

    // GET: Mesas/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mesa = await _context.Mesas
            .Include(m => m.Restaurante)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (mesa == null)
        {
            return NotFound();
        }

        return View(mesa);
    }

    // POST: Mesas/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var mesa = await _context.Mesas.FindAsync(id);
        if (mesa != null)
        {
            _context.Mesas.Remove(mesa);
            await _context.SaveChangesAsync();

            // Reorganizar los números de mesa
            var mesas = await _context.Mesas.OrderBy(m => m.NumeroMesa).ToListAsync();
            for (int i = 0; i < mesas.Count; i++)
            {
                mesas[i].NumeroMesa = i + 1;
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool MesaExists(int id)
    {
        return _context.Mesas.Any(e => e.Id == id);
    }
}
