/// <summary>
/// Summary description for UserManager
/// </summary>
public class UserManager
{

    public UserManager()
	{
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary>
    /// Authenticates user from the system. A hard-coded logic is used for demonstration
    /// </summary>
    /// <param name="UserName"></param>
    /// <param name="Password"></param>
    /// <returns></returns>
    public static WebUser AuthenticateUser(string UserName, string Password)
    {
        WebUser user = new WebUser();
        ESIdentity entity = new ESIdentity();
       var userData = entity.GetUsersBy(UserName, Password);

        user.UniqueId = userData.Id;
        user.UserName = userData.UserName;

        if (user != null)
        {
            user.Token = Utility.GetGuidHash();
        }

        return user;
    }

    /// <summary>
    /// Retrieves a user form the system. A hard-coded logic is used for demonstration
    /// </summary>
    /// <param name="UniqueId"></param>
    /// <returns></returns>
    public static WebUser GetWebUserByUniqueId(string UniqueId)
    {
        
        ESIdentity entity = new ESIdentity();
        var userData = entity.GetUsersByID(UniqueId);
        WebUser user = new WebUser();
        user.UniqueId = userData.Id;
        user.UserName = userData.UserName;
             

        return user;
    }
}
