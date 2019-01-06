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
            if (CheckToken(Authorization) == true)
            {
                var clazzs = _context.Clazz.ToList();
                return new JsonResult(clazzs);
            }
            return Unauthorized();
        }

        public IActionResult Details([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int clazzId)
        {
            if (CheckToken(Authorization) == true)
            {
                var clazzSubject = _context.ClazzSubject
                     .Where(cs => cs.ClazzId == clazzId)
                     .Include(cs => cs.Subject)
                     .ToList();
                var studentClazz = _context.StudentClazz
                    .Where(sc => sc.ClazzId == clazzId)
                    .Include(sc => sc.Account)
                    .ThenInclude(a => a.GeneralInformation)
                    .Include(sc => sc.Account)
                    .ThenInclude(a => a.RoleAccounts)
                    .ThenInclude(ra => ra.Role)
                    .ToList();
                var result = new { studentClazz, clazzSubject };
                return new JsonResult(result);
            }
            return Unauthorized();
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

        [HttpPost]
        public IActionResult AddStudents([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int[] studentIds)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var clazzId = studentIds.Last();
                var clazz = _context.Clazz.Find(clazzId);
                for (var i = 0; i < studentIds.Count() - 1; i++)
                {
                    var existedStudenClazz = _context.StudentClazz.Find(studentIds[i], clazzId);
                    if (existedStudenClazz != null)
                    {
                        continue;
                    }
                    var student = _context.Account.Find(studentIds[i]);
                    StudentClazz studentClazz = new StudentClazz
                    {
                        Account = student,
                        Clazz = clazz
                    };
                    _context.Add(studentClazz);
                }
                _context.SaveChanges();
                return new JsonResult(studentIds);
            }
            return Unauthorized();
        }

        public IActionResult AddSubjects([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int[] subjectIds)
        {
            if(CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var clazzId = subjectIds.Last();
                var clazz = _context.Clazz.Find(clazzId);
                for(var i = 0; i < subjectIds.Count() - 1; i++)
                {
                    var subject = _context.Subject.Find(subjectIds[i]);
                    ClazzSubject clazzSubject = new ClazzSubject {
                        Clazz = clazz,
                        Subject = subject
                    };
                    _context.Add(clazzSubject);
                }
                _context.SaveChanges();
                return new JsonResult(subjectIds);
            }
            return Unauthorized();
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
