﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

namespace obligatorio2024.Controllers
{
    public class ReseñaController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public ReseñaController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: Reseña
        public async Task<IActionResult> Index()
        {
            var obligatorio2024Context = _context.Reseñas.Include(r => r.Cliente).Include(r => r.Restaurante);
            return View(await obligatorio2024Context.ToListAsync());
        }

        // GET: Reseña/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseñas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reseña == null)
            {
                return NotFound();
            }

            return View(reseña);
        }

        // GET: Reseña/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id");
            return View();
        }

        // POST: Reseña/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,RestauranteId,Puntaje,Comentario,FechaReseña")] Reseña reseña)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reseña);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reseña.ClienteId);
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id", reseña.RestauranteId);
            return View(reseña);
        }

        // GET: Reseña/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseñas.FindAsync(id);
            if (reseña == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reseña.ClienteId);
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id", reseña.RestauranteId);
            return View(reseña);
        }

        // POST: Reseña/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,RestauranteId,Puntaje,Comentario,FechaReseña")] Reseña reseña)
        {
            if (id != reseña.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reseña);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReseñaExists(reseña.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reseña.ClienteId);
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id", reseña.RestauranteId);
            return View(reseña);
        }

        // GET: Reseña/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseñas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reseña == null)
            {
                return NotFound();
            }

            return View(reseña);
        }

        // POST: Reseña/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reseña = await _context.Reseñas.FindAsync(id);
            if (reseña != null)
            {
                _context.Reseñas.Remove(reseña);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReseñaExists(int id)
        {
            return _context.Reseñas.Any(e => e.Id == id);
        }

        // Nueva acción para mostrar el formulario de búsqueda y las reseñas
        public async Task<IActionResult> BuscarReseñas(int? id)
        {
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Dirección");

            if (id == null)
            {
                return View();
            }

            var restaurante = await _context.Restaurantes
                .Include(r => r.Reseñas)
                .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurante == null)
            {
                return NotFound();
            }

            return View(restaurante.Reseñas);
        }
    }
}
