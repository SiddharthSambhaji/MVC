using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESIdentityDataModel;
/// <summary>
/// Summary description for ESIdentity
/// </summary>
public class ESIdentity
{

    public IEnumerable<AspNetUser> GetAllUsers()
    {
        using (ESIdentityDBEntities entities= new ESIdentityDBEntities())
        {
            return entities.AspNetUsers.ToList();
        }
    }

    public AspNetUser GetUsersBy(string UserName, string PasswordHash)
    {
        using (ESIdentityDBEntities entities = new ESIdentityDBEntities())
        {
            return entities.AspNetUsers.FirstOrDefault(user => user.UserName == UserName && user.PasswordHash == PasswordHash);
        }
    }

    public AspNetUser GetUsersByID(string ID)
    {
        using (ESIdentityDBEntities entities = new ESIdentityDBEntities())
        {
            return entities.AspNetUsers.FirstOrDefault(user => user.Id == ID);
        }
    }

    //public ESIdentity()
    //{
    //    //
    //    // TODO: Add constructor logic here
    //    //
    //}
}