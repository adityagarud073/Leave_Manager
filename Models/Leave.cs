// namespace LeaveManagementSystem.Models;

// public class Leave
// {
//     public int Id { get; set; }

//     public DateTime FromDate { get; set; }

//     public DateTime ToDate { get; set; }

//     public string Reason { get; set; }

//     public string Status { get; set; }

//     public int UserId { get; set; }
// }

namespace LeaveManagementSystem.Models;

public class Leave
{
    public int Id { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Reason { get; set; }

    public string Status { get; set; }

    public int UserId { get; set; }

    public User User { get; set; }
}
