using EMSystemTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMSystemTask.Controllers
{
    public class EmployeeLayoutController : Controller
    {
        private readonly EmsDbContext _emsDbContext;
        public EmployeeLayoutController(EmsDbContext emsDbContext)
        {
            _emsDbContext = emsDbContext;
        }

        public IActionResult Index()
        {
            int id = (int)HttpContext.Session.GetInt32("EmployeeId");
            var leaveByUserId=_emsDbContext.LeaveRequests.Where(l => l.EmployeeId == id)
                .Include(l=>l.LeaveType)
                .OrderByDescending(l=>l.StartDate)
                .ToList();
            ViewBag.idForEmployee = id;
            return View(leaveByUserId);
        }

        public IActionResult CreateLeave()
        {
            //ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId");
            ViewData["LeaveTypeId"] = new SelectList(_emsDbContext.LeaveTypes, "LeaveTypeId", "LeaveTypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLeave([Bind("EmployeeId,LeaveTypeId,StartDate,EndDate,Status,RequestDate")] LeaveRequest leaveRequest)
        {
            if (leaveRequest.StartDate >= leaveRequest.EndDate)
            {
                ModelState.AddModelError(string.Empty, "Invalid Date: Start Date must be before End Date.");
            }
            var getIdBySession = HttpContext.Session.GetInt32("EmployeeId");

            leaveRequest.EmployeeId = getIdBySession.Value;
            

            if (!ModelState.IsValid)
            {
                ViewData["LeaveTypeId"] = new SelectList(_emsDbContext.LeaveTypes, "LeaveTypeId", "LeaveTypeName", leaveRequest.LeaveTypeId);
                return View(leaveRequest);
            }

            _emsDbContext.Add(leaveRequest);
            await _emsDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
