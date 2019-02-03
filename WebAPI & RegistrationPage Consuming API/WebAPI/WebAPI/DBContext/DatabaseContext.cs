/// <summary>
/// Author : Siddharth G. Sambhaji
/// </summary>
namespace WebAPI.DBContext
{
    using System.Data.Entity;
    using WebAPI.Models;
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DefaultConnection") { }

        public DbSet<Employee> Employees { get; set; }
    }
}