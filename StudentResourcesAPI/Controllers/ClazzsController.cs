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
        public IActionResult Index([FromHeader] string Authorization)
        {
            if (CheckPermission(Authorization))
            {
                var clazzs = _context.Clazz.ToList();
                return new JsonResult(clazzs);
            }
            return Unauthorized();
        }

        // GET: Clazzs/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ListStudentClazz = _context.StudentClazz
                .Where(sc => sc.ClazzId == id)
                .Include(sc => sc.Account)
                .ThenInclude(a => a.GeneralInformation)
                .ToList();
            var subjects = _context.Subject.ToList();
            var grades = _context.Grade.ToList();
            if (ListStudentClazz == null)
            {
                return NotFound();
            }
            ViewData["subjects"] = subjects;
            ViewData["grades"] = grades;
            return View(ListStudentClazz);
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
        public IActionResult Create([FromBody]Clazz clazz, [FromHeader] string Authorization, [FromHeader] string Role)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                _context.Add(clazz);
                _context.SaveChanges();
                return Ok();
            }
            return Unauthorized();
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
            //return View(clazz);
            return Ok();
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

        public IActionResult AddStudents(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clazz = _context.Clazz.Find(id);
            if (clazz == null)
            {
                return NotFound();
            }
            var students = _context.Account
                .Where(a => a.RoleAccounts.Any(ra => ra.RoleId == 2))
                .Include(a => a.GeneralInformation)
                .ToList();
            ViewData["students"] = students;
            return View(clazz);
        }

        [HttpPost]
        public async Task<IActionResult> SaveStudents(int? clazzId, int[] accountIds)
        {
            var clazz = _context.Clazz.Find(clazzId);
            foreach (var id in accountIds)
            {
                var existedStudenClazz = _context.StudentClazz.Find(id, clazzId);
                if(existedStudenClazz != null)
                {
                    continue;
                }
                var student = _context.Account.Find(id);
                StudentClazz studentClazz = new StudentClazz
                {
                    Clazz = clazz,
                    Account = student
                };
                _context.Add(studentClazz);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public bool CheckToken(string accessToken)
        {
            var credential = _context.Credential.SingleOrDefault(t => t.AccessToken == accessToken);
            if (credential != null && credential.IsValid())
            {
                return true;
            }
            return false;
        }

        public bool CheckPermission(string role)
        {
            if (role.Split("#").Contains("Employee"))
            {
                return true;
            }
            return false;
        }
    }
}
