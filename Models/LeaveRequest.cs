using System;
using System.Collections.Generic;

namespace EMSystemTask.Models;

public partial class LeaveRequest
{
    public int LeaveRequestId { get; set; }

    public int EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Status { get; set; }

    public DateTime? RequestDate { get; set; }

    public virtual Employee? Employee { get; set; } = null!;

    public virtual LeaveType? LeaveType { get; set; } = null!;
}
