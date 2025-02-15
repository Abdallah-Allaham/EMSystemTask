using System.Linq;
using EMSystemTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace EMSystemTask.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private readonly EmsDbContext _emsDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LoginAndRegisterController(EmsDbContext emsDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _emsDbContext = emsDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserInfo userInfo)
        {
            //check if the username is alredy existing in DB by using AnyAsync 
            bool usernameExists = await _emsDbContext.Users.AnyAsync(e => e.Username == userInfo.Username);

            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Invalid UserName or Username is Existing");
            }

            //check if the email is alredy existing in DB by using AnyAsync 
            bool emailExists = await _emsDbContext.Users.AnyAsync(e => e.Email == userInfo.Email);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Invalid Email or Email is Existing");
            }

            if (ModelState.IsValid)
            {
                // Validate Image Upload
                //if (userInfo.imageFile != null)
                //{
                //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                //    var fileExtension = Path.GetExtension(userInfo.imageFile.FileName).ToLower();

                //    if (!allowedExtensions.Contains(fileExtension))
                //    {
                //        ModelState.AddModelError("imageFile", "Only .jpg, .jpeg, and .png files are allowed.");
                //        return View(userInfo);
                //    }

                //    // Generate file name and path
                //    string fileName = Guid.NewGuid().ToString()+"_" + fileExtension;
                //    string filePath = Path.Combine(wwwRootPath, "Images", fileName);

                //    // Save image file
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await userInfo.imageFile.CopyToAsync(stream);
                //    }

                //    userInfo.ProfileImage = "/Images/" + fileName;
                //}
                if (userInfo.imageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + userInfo.imageFile.FileName;//if you need to give unique name use guid
                    string path = Path.Combine(wwwRootPath + "/Images/" + imageName);


                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await userInfo.imageFile.CopyToAsync(fileStream);
                    }
                    userInfo.ProfileImage = imageName;//to store image name in DB
                }

                User user = new()
                {
                    Username=userInfo.Username,
                    Email=userInfo.Email,
                    PasswordHash=userInfo.PasswordHash,
                    ProfileImage=userInfo.ProfileImage,
                    RoleId=3
                    
                };

                _emsDbContext.Add(user);
                await _emsDbContext.SaveChangesAsync();


                Employee employee = new()
                {
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    Email = userInfo.Email,
                    PhoneNumber = userInfo.PhoneNumber,
                    HireDate = DateOnly.FromDateTime(DateTime.Now),
                    Salary=400,
                    UserId=user.UserId,
                    ManageBy=13
                };

                _emsDbContext.Add(employee);
                await _emsDbContext.SaveChangesAsync();

                return RedirectToAction("Login", "LoginAndRegister");
            }
            return View(userInfo);

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            var userCheck = _emsDbContext.Users.FirstOrDefault(u => u.Username == user.Username);
            
            if (userCheck == null)
            {
                ModelState.AddModelError("username", "Invalid username ");
                return View();
            }

            bool isPassValid = user.PasswordHash == userCheck.PasswordHash;
            
            if(!isPassValid)
            {
                UserLogin userLoginF = new()
                {
                    UserId = userCheck.UserId,
                    LoginTime = DateTime.Now,
                    Status = "Failed"
                };

                _emsDbContext.Add(userLoginF);
                await _emsDbContext.SaveChangesAsync();

                ModelState.AddModelError("PasswordHash", "Invalid password");
                return View();
            }

            UserLogin userLogin = new()
            {
                UserId = userCheck.UserId,
                LoginTime = DateTime.Now,
                Status = "Success",
            };

            _emsDbContext.Add(userLogin);
            await _emsDbContext.SaveChangesAsync();

            switch (userCheck.RoleId)
            {
                case 1:
                    return RedirectToAction("Index", "Admin");

                case 2:
                    var idForManager = _emsDbContext.Employees.FirstOrDefault(e => e.UserId == userCheck.UserId);
                    if (idForManager != null)
                    {
                        HttpContext.Session.SetInt32("ManagerId", idForManager.EmployeeId);
                        return RedirectToAction("Index", "Manager");
                    }
                    else
                    {
                        ModelState.AddModelError("username", "Invalid username or password");
                        return View();
                    }
                case 3:
                    var idForEmployee = _emsDbContext.Employees.FirstOrDefault(e=>e.UserId==userCheck.UserId);
                    if (idForEmployee != null)
                    {
                        HttpContext.Session.SetInt32("EmployeeId", idForEmployee.EmployeeId);
                        return RedirectToAction("Index", "EmployeeLayout");
                    }
                    else
                    {
                        ModelState.AddModelError("username", "Invalid username or password");
                        return View();
                    }
                default:
                    ModelState.AddModelError("username", "Invalid username or password");
                    return View();

            }
        }
    }
}
