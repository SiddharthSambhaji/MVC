﻿using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// A sample User entity class
/// </summary>
public class WebUser
{
    public WebUser()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string UniqueId { get; set; }
    public string Token { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    //public string Token { get; set; }
    //public string Id { get; set; }
    //public string Email { get; set; }
    //public bool   EmailConfirmed { get; set; }
    //public string PasswordHash { get; set; }
    //public string SecurityStamp { get; set; }
    //public string PhoneNumber { get; set; }
    //public bool   PhoneNumberConfirmed { get; set; }
    //public bool   TwoFactorEnabled { get; set; }
    //public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
    //public bool LockoutEnabled { get; set; }
    //public int AccessFailedCount { get; set; }
    ////public string UserName { get; set; }
}
