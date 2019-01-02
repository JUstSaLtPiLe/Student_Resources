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
    public class AccountsController : Controller
    {
        private readonly StudentResourcesContext _context;

        public AccountsController(StudentResourcesContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var accounts = _context.Account
                .Include(a => a.GeneralInformation)
                .Include(a => a.RoleAccounts)
                .ThenInclude(ra => ra.Role)
                .ToList();

            var roleAccount = _context.RoleAccount
                .Include(ra => ra.Role)
                .ToList();
            return View(accounts);
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .Include(a => a.GeneralInformation)
                .Include(a => a.RoleAccounts)
                .ThenInclude(ra => ra.Role)
                .Include(a => a.StudentClazzs)
                .ThenInclude(sc => sc.Clazz)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            var roles = _context.Role.ToList();
            ViewData["roles"] = roles;
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GeneralInformation")] Account account, int[] roleIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in roleIds)
                {
                    var role = _context.Role.Find(id);
                    RoleAccount roleAccount = new RoleAccount
                    {
                        Role = role,
                        Account = account
                    };
                    _context.Add(roleAccount);
                }
                _context.Add(account);
                _context.Add(account.GeneralInformation);
                await _context.SaveChangesAsync();
                account.RollNumber = "B19APTECH" + account.AccountId.ToString("D4");
                account.Password = account.GeneralInformation.Dob.ToString();
                account.EncryptPassword(account.Password);
                _context.Update(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = _context.Account
                .Include(a => a.RoleAccounts)
                .Include(a => a.GeneralInformation)
                .Include(a => a.StudentClazzs)
                .ThenInclude(sc => sc.Clazz)
                .SingleOrDefault(a => a.AccountId == id);
            var roles = _context.Role.ToList();
            var clazzs = _context.Clazz.ToList();
            if (account == null)
            {
                return NotFound();
            }
            ViewData["roles"] = roles;
            ViewData["clazzs"] = clazzs;
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,RollNumber,Password,Salt,CreatedAt,UpdatedAt,Status")] Account account, int[] roleIds, int[] clazzIds)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(roleIds != null)
                    {
                        var OldRoleAccount = _context.RoleAccount.Where(ora => ora.AccountId == id);
                        _context.RoleAccount.RemoveRange(OldRoleAccount);
                    }
                   if(clazzIds != null)
                    {
                        var OldStudentClazz = _context.StudentClazz.Where(osc => osc.AccountId == id);
                        _context.StudentClazz.RemoveRange(OldStudentClazz);
                    }
                    foreach (var roleId in roleIds)
                    {
                        var role = _context.Role.Find(roleId);
                        RoleAccount roleAccount = new RoleAccount
                        {
                            Role = role,
                            Account = account
                        };
                        _context.Update(roleAccount);
                    }
                    foreach (var clazzId in clazzIds)
                    {
                        var clazz = _context.Clazz.Find(clazzId);
                        StudentClazz studentClazz = new StudentClazz
                        {
                            Clazz = clazz,
                            Account = account
                        };
                        _context.Update(studentClazz);
                    }
                    account.RollNumber = "B19APTECH" + account.AccountId.ToString("D4");
                    account.EncryptPassword(account.Password);
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.Include(a => a.GeneralInformation)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Account.FindAsync(id);
            var DeleteRoleAccount = _context.RoleAccount.Where(dra => dra.AccountId == id);
            var DeleteStudenClazz = _context.StudentClazz.Where(dtc => dtc.AccountId == id);
            _context.RoleAccount.RemoveRange(DeleteRoleAccount);
            _context.StudentClazz.RemoveRange(DeleteStudenClazz);
            _context.Account.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Account.Any(e => e.AccountId == id);
        }
    }
}
