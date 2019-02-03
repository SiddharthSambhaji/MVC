using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebAPI.Models;

namespace WebAPI.DBContext
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            base.Seed(context);

            Employee employee = new Employee
            {
                FirstName = "Siddharth",
                LastName = "Sambhaji",
                Gender = "Male",
                Address = "Goa",
                Phone = "9673673940",
                Email = "siddharths@arstraffic.com",
                Password = "Siddharth@21"
            };
            context.Employees.Add(employee);
            context.SaveChanges();
        }
    }
}