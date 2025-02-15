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
    public class UsersController : Controller
    {
        private readonly EmsDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(EmsDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var emsDbContext = _context.Users.Include(u => u.Role);
            return View(await emsDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Email,PasswordHash,imageFile,RoleId,CreatedAt")] User user)
        {
            //check if the username is alredy existing in DB by using AnyAsync 
            bool usernameExists = await _context.Users.AnyAsync(e => e.Username == user.Username);

            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Invalid UserName or Username is Existing");
            }

            //check if the email is alredy existing in DB by using AnyAsync 
            bool emailExists = await _context.Users.AnyAsync(e => e.Email == user.Email);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Invalid Email or Email is Existing");
            }

            if (user.imageFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;//to access w3root path
                string imageName = Guid.NewGuid().ToString() + "_" + user.imageFile.FileName;//if you need to give unique name use guid
                string path = Path.Combine(wwwRootPath + "/Images/" + imageName);


                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await user.imageFile.CopyToAsync(fileStream);
                }
                user.ProfileImage = imageName;//to store image name in DB
            }


            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,PasswordHash,imageFile,RoleId,CreatedAt")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // ✅ التحقق من أن اسم المستخدم غير مكرر باستثناء المستخدم الحالي
            bool usernameExists = await _context.Users.AnyAsync(e => e.Username == user.Username && e.UserId != id);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "This username is already taken.");
            }

            // ✅ التحقق من أن البريد الإلكتروني غير مكرر باستثناء المستخدم الحالي عن طريق اضافة شرط ثاني الي هو الايدي
            bool emailExists = await _context.Users.AnyAsync(e => e.Email == user.Email && e.UserId != id);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already in use.");
            }

            // ✅ معالجة الصورة إذا تم رفع صورة جديدة فقط
            if (user.imageFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadPath = Path.Combine(wwwRootPath, "Images");

                // حذف الصورة القديمة إذا كانت موجودة
                if (!string.IsNullOrEmpty(existingUser.ProfileImage))
                {
                    string oldImagePath = Path.Combine(uploadPath, existingUser.ProfileImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // ✅ حفظ الصورة الجديدة
                string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(user.imageFile.FileName);
                string newImagePath = Path.Combine(uploadPath, imageName);

                using (var fileStream = new FileStream(newImagePath, FileMode.Create))
                {
                    await user.imageFile.CopyToAsync(fileStream);
                }
                user.ProfileImage = imageName;
            }
            else
            {
                // ✅ الاحتفاظ بالصورة القديمة إذا لم يتم رفع صورة جديدة
                user.ProfileImage = existingUser.ProfileImage;
            }

            // ✅ التحقق من صحة النموذج قبل تحديث البيانات
            if (ModelState.IsValid)
            {
                try
                {
                    // ✅ تحديث الحقول الضرورية فقط (تجنب تغيير `PasswordHash` إلا إذا تم تعديلها)
                    existingUser.Username = user.Username;
                    existingUser.Email = user.Email;
                    existingUser.RoleId = user.RoleId;
                    existingUser.ProfileImage = user.ProfileImage;
                    existingUser.CreatedAt = user.CreatedAt; // لا تحتاج إلى تغييره إلا إذا كنت تريده أن يكون قابلًا للتعديل

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
