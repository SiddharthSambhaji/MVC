using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for LoggedInUser
/// </summary>
namespace ViewModels
{
    public class LoggedInUser
    {
        public long UserId { get; set; }

        public string IdentityId { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string RoleId { get; set; }

        public string RoleLevelId { get; set; }

        public bool Reset { get; set; }
        public bool isActive { get; set; }

        public string Token { get; set; }


    }
}