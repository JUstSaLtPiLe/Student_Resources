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
                .Where(a => a.Status != AccountStatus.Deactive)
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
                var roles = _context.Role
                    .Where(r => r.Status != RoleStatus.Deactive)
                    .ToList();
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
                    account.Password = account.GeneralInformation.Dob.ToString("ddMMyy");
                    account.EncryptPassword(account.Password);
                    _context.Update(account);
                    _context.SaveChanges();
                    return new JsonResult(account.GeneralInformation.Dob.ToString("ddMMyy"));
                }
                //return new JsonResult(generalInfoWithRoles);
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
                var clazz = _context.Clazz.Find(clazzId);
                var result = new { studentClazz, clazzSubject ,clazz };
                return new JsonResult(result);
            }
            return Unauthorized();
        }

        public IActionResult StudentDetails([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int studentId)
        {
            if (CheckToken(Authorization) == true)
            {
                var student = _context.Account
                     .Where(a => a.AccountId == studentId)
                     .Include(a => a.GeneralInformation)
                     .Include(a => a.RoleAccounts)
                     .ToList();
                return new JsonResult(student);
            }
            return Unauthorized();
        }

        public IActionResult EditAccount([FromBody] GeneralInformationWithRoles generalInfoWithRoles, [FromHeader] string Authorization)
        {
            if (CheckToken(Authorization) == true)
            {
                var account = _context.Account.Find(generalInfoWithRoles.AccountId);
                var generalInfo = _context.GeneralInformation.Find(generalInfoWithRoles.AccountId);
                account.RollNumber = "B19APTECH" + account.AccountId.ToString("D4");
                generalInfo.Phone = generalInfoWithRoles.GeneralInformation.Phone;
                generalInfo.Address = generalInfoWithRoles.GeneralInformation.Address;
                generalInfo.Email = generalInfoWithRoles.GeneralInformation.Email;
                account.UpdatedAt = DateTime.Today;
                if (generalInfoWithRoles.Password != null)
                {
                    account.EncryptPassword(generalInfoWithRoles.Password);
                }
                if (generalInfoWithRoles.RoleIds != null)
                {
                    var OldRoleAccount = _context.RoleAccount.Where(ora => ora.AccountId == generalInfoWithRoles.AccountId);
                    _context.RoleAccount.RemoveRange(OldRoleAccount);
                }
                foreach (var roleId in generalInfoWithRoles.RoleIds)
                {
                    var role = _context.Role.Find(roleId);
                    RoleAccount roleAccount = new RoleAccount
                    {
                        Role = role,
                        Account = account
                    };
                    _context.RoleAccount.Add(roleAccount);
                }
                _context.Account.Update(account);
                _context.GeneralInformation.Update(generalInfo);
                _context.SaveChanges();
                return new JsonResult(generalInfoWithRoles);
            }
            return Unauthorized();
        }

        public IActionResult EditClazz([FromBody] Clazz clazz, [FromHeader] string Authorization, [FromHeader] string Role)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role))
            {
                var existedClazz = _context.Clazz.Find(clazz.ClazzId);
                if(existedClazz != null)
                {
                    existedClazz.Name = clazz.Name;
                    existedClazz.Teacher = clazz.Teacher;
                    existedClazz.Status = clazz.Status;
                    existedClazz.UpdateAt = DateTime.Today;
                    _context.Update(existedClazz);
                    _context.SaveChanges();
                    return Ok();
                }
                return NoContent();
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
                    var existedClazzSubject = _context.ClazzSubject.Find(clazzId, subjectIds[i]);
                    if(existedClazzSubject != null)
                    {
                        continue;
                    }
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
                    var existedStudentClazz = _context.StudentClazz.Find(studentIds[i], clazzId);
                    if (existedStudentClazz != null)
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
                    var existedGrade = _context.Grade.Find(grade.AccountId, grade.SubjectId);
                    if(existedGrade != null)
                    {
                        continue;
                    }
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

        public IActionResult DeleteClazz([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int clazzId)
        {
            if(CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var clazz =  _context.Clazz.Find(clazzId);
                var studentClazzs = _context.StudentClazz.Where(sc => sc.ClazzId == clazzId);
                var clazzSubjects = _context.ClazzSubject.Where(cs => cs.ClazzId == clazzId);
                if(clazz != null)
                {
                    if(studentClazzs != null)
                    {
                        foreach(var studentClazz in studentClazzs)
                        {
                            _context.StudentClazz.Remove(studentClazz);
                        }
                    }
                    if (clazzSubjects != null)
                    {
                        foreach (var clazzSubject in clazzSubjects)
                        {
                            _context.ClazzSubject.Remove(clazzSubject);
                        }
                    }
                    _context.Clazz.Remove(clazz);
                    _context.SaveChanges();
                    return Ok();
                }
                return NoContent();
            }
            return Unauthorized();
        }

        public IActionResult DeleteStudentFromClazz([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] StudentClazz studentClazz)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var studentClazzs = _context.StudentClazz.Find(studentClazz.AccountId , studentClazz.ClazzId);
                if (studentClazzs != null)
                {
                    _context.Remove(studentClazzs);
                    _context.SaveChanges();
                    return Ok();
                }
                return NoContent();
            }
            return Unauthorized();
        }

        public IActionResult DeleteSubjectFromClazz([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] ClazzSubject clazzSubject)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var clazzSubjects = _context.ClazzSubject.Find(clazzSubject.ClazzId, clazzSubject.SubjectId);
                if (clazzSubjects != null)
                {
                    _context.Remove(clazzSubjects);
                    _context.SaveChanges();
                    return Ok();
                }
                return NoContent();
            }
            return Unauthorized();
        }

        public IActionResult DeleteSubject([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int subjectId)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var subject = _context.Subject.Find(subjectId);
                var clazzSubjects = _context.ClazzSubject.Where(cs => cs.SubjectId == subjectId);
                if (subject != null)
                {
                    if (clazzSubjects != null)
                    {
                        foreach (var clazzSubject in clazzSubjects)
                        {
                            _context.ClazzSubject.Remove(clazzSubject);
                        }
                    }
                    _context.Subject.Remove(subject);
                    _context.SaveChanges();
                    return Ok();
                }
                return NoContent();
            }
            return Unauthorized();
        }

        public IActionResult DeleteAccount([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int accountId)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                var account = _context.Account.Find(accountId);
                var roleAccounts = _context.RoleAccount.Where(ra => ra.AccountId == accountId);
                var studentClazzs = _context.StudentClazz.Where(sc => sc.AccountId == accountId);
                var grades = _context.Grade.Where(sc => sc.AccountId == accountId);
                if (account != null)
                {
                    if (roleAccounts != null)
                    {
                        foreach (var roleAccount in roleAccounts)
                        {
                            _context.RoleAccount.Remove(roleAccount);
                        }
                    }
                    if (studentClazzs != null)
                    {
                        foreach (var studentClazz in studentClazzs)
                        {
                            _context.StudentClazz.Remove(studentClazz);
                        }
                    }
                    if (grades != null)
                    {
                        foreach (var grade in grades)
                        {
                            _context.Grade.Remove(grade);
                        }
                    }
                    _context.Account.Remove(account);
                    _context.SaveChanges();
                    return Ok();
                }
                return NoContent();
            }
            return Unauthorized();
        }

        public IActionResult GetStudenClazzs([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int studentId)
        {
            if(CheckToken(Authorization) == true)
            {
                var studentClazzs = _context.StudentClazz
                    .Where(sc => sc.AccountId == studentId)
                    .Include(sc => sc.Clazz)
                    .ToList();
                return new JsonResult(studentClazzs);
            }
            return Unauthorized();
        }

        public IActionResult GetStudenGrades([FromHeader] string Authorization, [FromHeader] string Role, [FromBody] int[] studentIds)
        {
            if (CheckToken(Authorization) == true)
            {
                var studentId = studentIds.Last();
                var grade = _context.Grade.Find(studentId, studentIds[0]);
                if(grade != null)
                {
                    return new JsonResult(grade);
                }
                return NoContent();
            }
            return Unauthorized();
        }

        public IActionResult EditGrades([FromHeader] string Authorization, [FromHeader] string Role, [FromBody]IEnumerable<Grade> grades)
        {
            if (CheckToken(Authorization) == true && CheckPermission(Role) == true)
            {
                foreach (var grade in grades)
                {
                    var existedGrade = _context.Grade.Find(grade.AccountId, grade.SubjectId);
                    if (existedGrade == null)
                    {
                        return NoContent();
                    }
                    existedGrade.AccountId = grade.AccountId;
                    existedGrade.SubjectId = grade.SubjectId;
                    existedGrade.AssignmentGrade = grade.AssignmentGrade;
                    existedGrade.PraticalGrade = grade.PraticalGrade;
                    existedGrade.TheoricalGrade = grade.TheoricalGrade;
                    existedGrade.UpdatedAt = DateTime.Today;
                    if (existedGrade.TheoricalGrade < 5)
                    {
                        existedGrade.TheoricalGradeStatus = GradeStatus.Failed;
                    }
                    if (existedGrade.AssignmentGrade < 5)
                    {
                        existedGrade.AssignmentGradeStatus = GradeStatus.Failed;
                    }
                    if (existedGrade.PraticalGrade < 5)
                    {
                        existedGrade.PraticalGradeStatus = GradeStatus.Failed;
                    }
                    _context.Update(existedGrade);
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
