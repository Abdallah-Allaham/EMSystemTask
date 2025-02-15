using System;
using System.Collections.Generic;

namespace EMSystemTask.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateOnly HireDate { get; set; }

    public decimal Salary { get; set; }

    public int UserId { get; set; }

    public int? DepartmentId { get; set; }

    public int? ManageBy {  get; set; }
    public virtual Department? Department { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual User? User { get; set; } = null!;


    public virtual Employee? Manager { get; set; }
}
