using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EMSystemTask.Models;

namespace EMSystemTask.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmsDbContext _context;

        public EmployeesController(EmsDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var emsDbContext = _context.Employees.Include(e => e.Department).Include(e => e.User);
            return View(await emsDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["Manager"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,HireDate,Salary,UserId,DepartmentId,ManageBy")] Employee employee)
        {
            //check if the email is alredy existing in DB by using AnyAsync 
            bool emailExists = await _context.Employees.AnyAsync(e => e.Email == employee.Email);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Invalid Email or Email is Existing");
            }


            if (employee.PhoneNumber!=null)
            {
                bool validPhoneNumber = employee.PhoneNumber.StartsWith("079") || employee.PhoneNumber.StartsWith("078") || employee.PhoneNumber.StartsWith("077");
                if (!validPhoneNumber)
                {
                    ModelState.AddModelError("PhoneNumber", "Must Start With 079 or 078 or 077");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["Manager"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ManageBy);
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", employee.UserId);
                return View(employee);

            }

            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["Manager"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId",employee.ManageBy);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", employee.UserId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FirstName,LastName,Email,PhoneNumber,HireDate,Salary,UserId,DepartmentId,ManageBy")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            //check if the email is alredy existing except(الحالي) the current user in DB by using AnyAsync  
            bool emailExists = await _context.Employees.AnyAsync(e => e.Email == employee.Email && e.EmployeeId != id);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Invalid Email or Email is Existing");
            }


            if (employee.PhoneNumber != null)
            {
                bool validPhoneNumber = employee.PhoneNumber.StartsWith("079") || employee.PhoneNumber.StartsWith("078") || employee.PhoneNumber.StartsWith("077");
                if (!validPhoneNumber)
                {
                    ModelState.AddModelError("PhoneNumber", "Must Start With 079 or 078 or 077");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["Manager"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ManageBy);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", employee.UserId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
