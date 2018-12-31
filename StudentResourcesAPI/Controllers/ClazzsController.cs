using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentResourcesAPI.Data;
using StudentResourcesAPI.Models;

namespace StudentResourcesAPI.Controllers
{
    public class ClazzsController : Controller
    {
        private readonly StudentResourcesContext _context;

        public ClazzsController(StudentResourcesContext context)
        {
            _context = context;
        }

        // GET: Clazzs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clazz.ToListAsync());
        }

        // GET: Clazzs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clazz = await _context.Clazz
                .FirstOrDefaultAsync(m => m.ClazzId == id);
            if (clazz == null)
            {
                return NotFound();
            }

            return View(clazz);
        }

        // GET: Clazzs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clazzs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Teacher,Status")] Clazz clazz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clazz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clazz);
        }

        // GET: Clazzs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clazz = await _context.Clazz.FindAsync(id);
            if (clazz == null)
            {
                return NotFound();
            }
            return View(clazz);
        }

        // POST: Clazzs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Teacher,Status")] Clazz clazz)
        {
            if (id != clazz.ClazzId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clazz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClazzExists(clazz.ClazzId))
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
            return View(clazz);
        }

        // GET: Clazzs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clazz = await _context.Clazz
                .FirstOrDefaultAsync(m => m.ClazzId == id);
            if (clazz == null)
            {
                return NotFound();
            }

            return View(clazz);
        }

        // POST: Clazzs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clazz = await _context.Clazz.FindAsync(id);
            _context.Clazz.Remove(clazz);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClazzExists(int id)
        {
            return _context.Clazz.Any(e => e.ClazzId == id);
        }
    }
}
