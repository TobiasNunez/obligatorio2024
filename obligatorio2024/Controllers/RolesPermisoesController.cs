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
    public class RolesPermisoesController : Controller
    {
        private readonly Obligatorio2024Context _context;

        public RolesPermisoesController(Obligatorio2024Context context)
        {
            _context = context;
        }

        // GET: RolesPermisoes
        public async Task<IActionResult> Index()
        {
            var obligatorio2024Context = _context.RolesPermisos.Include(r => r.Rol);
            return View(await obligatorio2024Context.ToListAsync());
        }

        // GET: RolesPermisoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolesPermiso = await _context.RolesPermisos
                .Include(r => r.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rolesPermiso == null)
            {
                return NotFound();
            }

            return View(rolesPermiso);
        }

        // GET: RolesPermisoes/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: RolesPermisoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RolId,Permiso")] RolesPermiso rolesPermiso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rolesPermiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Id", rolesPermiso.RolId);
            return View(rolesPermiso);
        }

        // GET: RolesPermisoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolesPermiso = await _context.RolesPermisos.FindAsync(id);
            if (rolesPermiso == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Id", rolesPermiso.RolId);
            return View(rolesPermiso);
        }

        // POST: RolesPermisoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RolId,Permiso")] RolesPermiso rolesPermiso)
        {
            if (id != rolesPermiso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rolesPermiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolesPermisoExists(rolesPermiso.Id))
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
            ViewData["RolId"] = new SelectList(_context.Roles, "Id", "Id", rolesPermiso.RolId);
            return View(rolesPermiso);
        }

        // GET: RolesPermisoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rolesPermiso = await _context.RolesPermisos
                .Include(r => r.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rolesPermiso == null)
            {
                return NotFound();
            }

            return View(rolesPermiso);
        }

        // POST: RolesPermisoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rolesPermiso = await _context.RolesPermisos.FindAsync(id);
            if (rolesPermiso != null)
            {
                _context.RolesPermisos.Remove(rolesPermiso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolesPermisoExists(int id)
        {
            return _context.RolesPermisos.Any(e => e.Id == id);
        }
    }
}
