using System.Linq;
using EMSystemTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMSystemTask.Controllers
{
    public class ManagerController : Controller
    {
        private readonly EmsDbContext _emsDbContext;
        public ManagerController(EmsDbContext emsDbContext)
        {
            _emsDbContext = emsDbContext;
        }

        public IActionResult Index()
        {
            int id = (int)HttpContext.Session.GetInt32("ManagerId");
            
            var employeeForManager = _emsDbContext.Employees.Where(e => e.ManageBy == id).ToList();
            var justId = employeeForManager.Select(e => e.EmployeeId).ToList();

            var LeaveForEmployee = _emsDbContext.LeaveRequests.Where(l => justId.Contains(l.EmployeeId))
                .Include(t => t.LeaveType)
                .ToList();
            return View(LeaveForEmployee);
        }

        public IActionResult Approve(int? id)
        {
            var leaveA = _emsDbContext.LeaveRequests.Find(id);

            if (leaveA != null) 
            {
                leaveA.Status = "Approved";
                _emsDbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Reject(int? id)
        {
            var leaveR = _emsDbContext.LeaveRequests.Find(id);

            if (leaveR != null)
            {
                leaveR.Status = "Rejected";
                _emsDbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Pending(int? id)
        {
            var leaveP = _emsDbContext.LeaveRequests.Find(id);

            if (leaveP != null)
            {
                leaveP.Status = "Pending";
                _emsDbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
