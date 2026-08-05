using Microsoft.EntityFrameworkCore;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Leave> Leaves { get; set; }
}