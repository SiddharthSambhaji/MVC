

namespace Assignment.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Employee
    {
        [Required(ErrorMessage = "First Name Required *")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last Name Required *")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Company Email address Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Valid Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }


        public string Gender { get; set; }

        [Required]
        [StringLength(15, MinimumLength =5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required]
        //[Compare("Password", ErrorMessage = "Please Re-enter Password Again")]
        //public string ComparedPassword { get; set; }

    }
}