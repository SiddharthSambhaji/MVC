using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.DBContext;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class EmployeeController : ApiController
    {
        //Creating Instance of DatabaseContext class  
        private DatabaseContext db = new DatabaseContext();

        //Creating a method to return Json data   
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                //Prepare data to be returned using Linq as follows  
                var result = from country in db.Employees 
                             select new
                             {
                                 country.EmployeeID,
                                 country.FirstName,
                                 country.LastName,
                                 country.Gender,
                                 country.Phone,
                                 country.Email,
                                 country.Address,
                             };
                return Ok(result);
            }
            catch (Exception)
            {
                //If any exception occurs Internal Server Error i.e. Status Code 500 will be returned  
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult InsertEmployee(Employee employee)
        {

            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            db.Employees.Add(employee);
            db.SaveChanges();
            return Ok();

        }
    }
}