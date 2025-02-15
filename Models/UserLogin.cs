using System;
using System.Collections.Generic;

namespace EMSystemTask.Models;

public partial class UserLogin
{
    public int LoginId { get; set; }

    public int UserId { get; set; }

    public DateTime? LoginTime { get; set; }

    public string? Status { get; set; }

    public virtual User? User { get; set; } = null!;
}
