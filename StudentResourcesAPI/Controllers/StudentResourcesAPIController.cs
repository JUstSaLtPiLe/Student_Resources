using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentResourcesAPI.Data;
using StudentResourcesAPI.Models;

namespace StudentResourcesAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentResourcesAPIController : ControllerBase
    {
        private readonly StudentResourcesContext _context;

        public StudentResourcesAPIController(StudentResourcesContext context)
        {
            _context = context;
        }

        public IActionResult Login([FromBody]Account account)
        {
            var existAccount = _context.Account
                .Include(a => a.GeneralInformation)
                .SingleOrDefault(a => a.RollNumber == account.RollNumber);
            if (existAccount != null)
            {
                var roleAccounts = _context.RoleAccount
                    .Include(ra => ra.Role)
                    .Where(ra => ra.AccountId == existAccount.AccountId);
                account.Salt = existAccount.Salt;
                account.EncryptPassword(account.Password);
                var roles = "";
                if (existAccount.Password == account.Password)
                {
                    foreach (var ra in roleAccounts)
                    {
                        roles += ra.Role.Name + "#";
                    }
                    var credential = _context.Credential.SingleOrDefault(cr => cr.OwnerId == existAccount.AccountId);
                    if (credential == null)
                    {
                        credential = new Credential(existAccount.AccountId);
                        _context.Credential.Add(credential);
                        _context.SaveChanges();
                    }
                    Response.StatusCode = 200;
                    var result = new { credential, roles, existAccount.GeneralInformation.Name };
                    return new JsonResult(result);
                }
                else
                {
                    return Unauthorized();
                }
            }
            return NoContent();
        }

        public IActionResult AccountsIndex([FromHeader] string Authorization)
        {
            if (CheckToken(Authorization) == true)
            {
                var employeeAccounts = _context.RoleAccount
                .Where(ra => ra.RoleId == 1)
                .Include(ra => ra.Account)
                .ThenInclude(a => a.GeneralInformation)
                .Include(ra => ra.Role)
                .ToList();
                var studentAccounts = _context.RoleAccount
                .Where(ra => ra.RoleId == 2)
                .Include(ra => ra.Account)
                .ThenInclude(a => a.GeneralInformation)
                .Include(ra => ra.Role)
                .ToList();
                var result = new { employeeAccounts, studentAccounts };
                return new JsonResult(result);
            }
            return Unauthorized();
        }

        public IActionResult ClazzsIndex([FromHeader] string Authorization)
        {
            if (CheckToken(Authorization) == true)
            {
                var clazzs = _context.Clazz.ToList();
                return new JsonResult(clazzs);
            }
            return Unauthorized();
        }

        public IActionResult SubjectsIndex([FromHeader] string Authorization, [FromHeader] string Role)
        {
            if (CheckToken(Authorization) == true)
            {
                var subjects = _context.Subject.ToList();
                return new JsonResult(subjects);
            }
            return Unauthorized();
        }

        [HttpGet]
        public IActionResult CreateAccount([FromHeader] string Authorization, [FromHeader] string Role)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var roles = _context.Role.ToList();
                return new JsonResult(roles);
            }
            return Unauthorized();
        }

        public IActionResult CreateAccount([FromBody] GeneralInformationWithRoles generalInfoWithRoles, [FromHeader] string Authorization, [FromHeader] string Role)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                if (ModelState.IsValid)
                {
                    Account account = new Account();
                    account.GeneralInformation = generalInfoWithRoles.GeneralInformation;
                    foreach (var id in generalInfoWithRoles.RoleIds)
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
                    _context.SaveChanges();
                    account.RollNumber = "B19APTECH" + account.AccountId.ToString("D4");
                    account.Password = account.GeneralInformation.Dob.ToString();
                    account.EncryptPassword(account.Password);
                    _context.Update(account);
                    _context.SaveChanges();
                }
                return new JsonResult(generalInfoWithRoles);
            }
            return Unauthorized();
        }

        public IActionResult CreateClazz([FromBody]Clazz clazz, [FromHeader] string Authorization, [FromHeader] string Role)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                _context.Add(clazz);
                _context.SaveChanges();
                return Ok();
            }
            return Unauthorized();
        }


        public IActionResult CreateSubject([FromBody]Subject subject, [FromHeader] string Authorization, [FromHeader] string Role)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role))
            {
                _context.Add(subject);
                _context.SaveChanges();
                return Ok();
            }
            return Unauthorized();
        }

        public IActionResult ClazzDetails([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int clazzId)
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

        public IActionResult AddSubjects([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int[] subjectIds)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var clazzId = subjectIds.Last();
                var clazz = _context.Clazz.Find(clazzId);
                for (var i = 0; i < subjectIds.Count() - 1; i++)
                {
                    var subject = _context.Subject.Find(subjectIds[i]);
                    ClazzSubject clazzSubject = new ClazzSubject
                    {
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

        public IActionResult AddGrades([FromBody]IEnumerable<Grade> grades, [FromHeader] string Authorization, [FromHeader] string Role)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                foreach (var grade in grades)
                {
                    grade.AccountId = grade.AccountId;
                    grade.SubjectId = grade.SubjectId;
                    if (grade.TheoricalGrade < 5)
                    {
                        grade.TheoricalGradeStatus = GradeStatus.Failed;
                    }
                    if (grade.AssignmentGrade < 5)
                    {
                        grade.AssignmentGradeStatus = GradeStatus.Failed;
                    }
                    if (grade.PraticalGrade < 5)
                    {
                        grade.PraticalGradeStatus = GradeStatus.Failed;
                    }
                    _context.Add(grade);
                }
                _context.SaveChanges();
                return Ok();
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
