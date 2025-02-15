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
    public class UserLoginsController : Controller
    {
        private readonly EmsDbContext _context;

        public UserLoginsController(EmsDbContext context)
        {
            _context = context;
        }

        // GET: UserLogins
        public async Task<IActionResult> Index()
        {
            var emsDbContext = _context.UserLogins.Include(u => u.User);
            return View(await emsDbContext.ToListAsync());
        }

        // GET: UserLogins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogins
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.LoginId == id);
            if (userLogin == null)
            {
                return NotFound();
            }

            return View(userLogin);
        }

        // GET: UserLogins/Create
        public IActionResult Create()
        {
            ViewBag.Status = new List<SelectListItem>
            {
                new SelectListItem { Value = "Success", Text = "Success" },
                new SelectListItem { Value = "Failed", Text = "Failed" }
            };


            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: UserLogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoginId,UserId,LoginTime,Status")] UserLogin userLogin)
        {

            if (!ModelState.IsValid)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", userLogin.UserId);
                
                return View(userLogin);
            }

            _context.Add(userLogin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: UserLogins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogins.FindAsync(id);
            if (userLogin == null)
            {
                return NotFound();
            }
            ViewBag.Status = new List<SelectListItem>
            {
                new SelectListItem { Value = "Success", Text = "Success" },
                new SelectListItem { Value = "Failed", Text = "Failed" }
            };
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", userLogin.UserId);
            return View(userLogin);
        }

        // POST: UserLogins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoginId,UserId,LoginTime,Status")] UserLogin userLogin)
        {

            if (id != userLogin.LoginId)
            {
                return NotFound();
            }

            /*if (ModelState.IsValid)
            {*/
                try
                {
                    _context.Update(userLogin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserLoginExists(userLogin.LoginId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", userLogin.UserId);
            return View(userLogin);
        }

        // GET: UserLogins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogins
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.LoginId == id);
            if (userLogin == null)
            {
                return NotFound();
            }

            return View(userLogin);
        }

        // POST: UserLogins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userLogin = await _context.UserLogins.FindAsync(id);
            if (userLogin != null)
            {
                _context.UserLogins.Remove(userLogin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserLoginExists(int id)
        {
            return _context.UserLogins.Any(e => e.LoginId == id);
        }
    }
}
